namespace Honours_Stage_Project.Services
{
    public interface ILuaStubValidationService
    {
        LuaValidationResult Validate(string luaScript, string stubFilePath);
    }
}