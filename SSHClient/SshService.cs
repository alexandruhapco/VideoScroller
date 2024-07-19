using Renci.SshNet;

namespace SSHClient;

public class SshService
{
    public static void SendFolder(string host, int port, string username, string password, string localFolderPath, string remoteFolderPath)
    {
        using var sftp = new SftpClient(host, port, username, password);

        Console.WriteLine($"Sftp {host}:{port} {username} connecting...");
        sftp.Connect();
        Console.WriteLine($"Sftp {host}:{port} {username} connected!");

        // Ensure the remote folder exists
        if (!sftp.Exists(remoteFolderPath))
        {
            sftp.CreateDirectory(remoteFolderPath);
            Console.WriteLine($"Remote folder created: {remoteFolderPath}");
        }

        // Get all files in the local folder
        var files = Directory.GetFiles(localFolderPath);
        int totalFiles = files.Length;
        int i = 0;

        foreach (var filePath in files)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open);
            var fileName = Path.GetFileName(filePath);
            var remoteFileName = $"{remoteFolderPath}/{fileName}";

            Console.WriteLine($"Upload started: {remoteFileName}");
            sftp.UploadFile(fileStream, remoteFileName);
            Console.WriteLine($"Upload finished: {remoteFileName}");

            // Set permissions to rwxrwx---
            sftp.ChangePermissions(remoteFileName, 0770);
            Console.WriteLine($"Permissions for {fileName} set: {0770}");
            
            Console.WriteLine($"Progress: {++i}/{totalFiles}");
        }

        sftp.Disconnect();
        Console.WriteLine($"Sftp disconnected!");
    }
}
