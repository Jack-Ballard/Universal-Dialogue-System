using System.Collections.Generic;

namespace Honours_Stage_Project.Services
{
    public interface ILuaStubValidationService
    {
        LuaValidationResult Validate(string luaScript);
        List<LuaValidationResult> ValidateLua(string luaScript);
    }
}