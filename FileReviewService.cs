using System.Security.Cryptography;

namespace AgentJeff;

/// <summary>
/// Creates a local, read-only file handoff summary. It never uploads, modifies,
/// launches, or deletes the selected file.
/// </summary>
public static class FileReviewService
{
    public static async Task<FileReviewResult> ReviewAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Choose a file first.", nameof(path));
        var info = new FileInfo(path);
        if (!info.Exists) throw new FileNotFoundException("The selected file no longer exists.", path);
        if ((info.Attributes & FileAttributes.Directory) != 0) throw new InvalidOperationException("Choose a file, not a folder.");
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 128, FileOptions.Asynchronous | FileOptions.SequentialScan);
        using var sha = SHA256.Create();
        var hash = await sha.ComputeHashAsync(stream, cancellationToken);
        return new FileReviewResult(info.Name, info.FullName, info.Length, info.Extension, Convert.ToHexString(hash), info.LastWriteTimeUtc);
    }
}

public sealed record FileReviewResult(string Name, string FullPath, long SizeBytes, string Extension, string Sha256, DateTime LastModifiedUtc)
{
    public string ToDisplayText() =>
        $"FILE REVIEW (LOCAL ONLY)\nName: {Name}\nType: {(string.IsNullOrEmpty(Extension) ? "Unknown" : Extension)}\nSize: {SizeBytes:N0} bytes\nLast modified (UTC): {LastModifiedUtc:yyyy-MM-dd HH:mm:ss}\nSHA-256: {Sha256}\nPath: {FullPath}\n\nNo file contents were uploaded or changed. A hash identifies these exact bytes; it does not prove a file is safe.";
}
