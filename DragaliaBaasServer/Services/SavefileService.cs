using DragaliaBaasServer.Models.Web;
using System.IO.Compression;

namespace DragaliaBaasServer.Services;

public class SavefileService : ISavefileService
{
    private readonly ILogger _logger;
    private readonly string _saveDirectory;

    public SavefileService(ILoggerFactory logger, IConfiguration configuration)
    {
        _logger = logger.CreateLogger("Savefile");
        _saveDirectory = configuration["SaveDirectory"] ?? "savefiles";

        if (!Directory.Exists(_saveDirectory))
            Directory.CreateDirectory(_saveDirectory);
    }

    public async Task<bool> SaveSavefile(WebUserAccount webAccount, Stream saveFile)
    {
        var filePath = Path.Join(_saveDirectory, $"{webAccount.Id}-save.json.gz");

        var tempBuffer = new byte[1];

        if (await saveFile.ReadAsync(tempBuffer) != 1 || tempBuffer[0] != '{')
        {
            _logger.LogWarning("Rejecting improper savefile upload for {wAccountName}.", webAccount.Username);
            return false;
        }

        await using (var fs = File.Open(filePath, FileMode.Create))
        {
            await using var deflateStream = new GZipStream(fs, CompressionLevel.Fastest);
            deflateStream.Write(tempBuffer);
            await saveFile.CopyToAsync(deflateStream);
        }

        _logger.LogInformation("Uploaded savefile for {wAccountName}.", webAccount.Username);
        webAccount.SavefileUploadedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return true;
    }

    public Stream? GetSavefile(WebUserAccount webAccount)
    {
        if (!webAccount.HasSavefile)
            return null;

        var fs = File.OpenRead(Path.Join(_saveDirectory, $"{webAccount.Id}-save.json.gz"));
        var inflateStream = new GZipStream(fs, CompressionMode.Decompress);
        return inflateStream;
    }

    public bool DeleteSavefile(WebUserAccount webAccount)
    {
        if (!webAccount.HasSavefile)
            return true;

        var filePath = Path.Join(_saveDirectory, $"{webAccount.Id}-save.json");
        if (File.Exists(filePath))
            File.Delete(filePath);

        webAccount.SavefileUploadedAt = 0;
        return true;
    }
}