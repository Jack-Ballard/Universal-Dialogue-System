using System.IO;

namespace Honours_Stage_Project.Services
{
    public class FileService : IFileService
    {
        public void WriteAllText(string path, string content) => File.WriteAllText(path, content);
        public void ReadAllText(string path, out string content) => content = File.ReadAllText(path);
    }
}
