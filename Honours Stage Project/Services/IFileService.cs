namespace Honours_Stage_Project.Services
{
    public interface IFileService
    {
        void WriteAllText(string path, string content);

        void ReadAllText(string path, out string content);
    }
}
