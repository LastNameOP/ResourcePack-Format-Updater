using System.IO;
using System.IO.Compression;
using System.Text;

static class ArchiveHandler
{
    public static string ReadFile(string path)
    {
        string archivePath = path;
        string packMCMETA;

        using (ZipArchive zipArchive = ZipFile.Open(archivePath, ZipArchiveMode.Update))
        {
            string fileName = "pack.mcmeta";
            using (MemoryStream stream = new MemoryStream())
            {
                var file = zipArchive.Entries.FirstOrDefault(x => x.Name == fileName)?.Open();
                if (file == null) { return string.Empty; }
                file.CopyTo(stream);
                file.Close();
                byte[] buffer = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(buffer, 0, buffer.Length);
                packMCMETA = Encoding.Default.GetString(buffer);
                stream.Close();
            }
            zipArchive.Entries.FirstOrDefault(x => x.Name == fileName)?.Delete();   
        }
        return packMCMETA;
    }

    public static void WriteFile(string packMCMETA, string path)
    {
        string archivePath = path;

        using (ZipArchive zipArchive = ZipFile.Open(archivePath, ZipArchiveMode.Update))
        {
            var file = zipArchive.CreateEntry("pack.mcmeta");
            using (StreamWriter writer = new StreamWriter(file.Open()))
            {
                writer.Write(packMCMETA);
            }
        }
    }
}

