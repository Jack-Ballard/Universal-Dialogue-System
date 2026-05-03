using System.Collections.Generic;
using Honours_Stage_Project.Models;

namespace Honours_Stage_Project.Services
{
    public interface IImportService
    {
        (List<NodeModel>, List<Connection>) ImportDialogue();

        LuaStubDefinition ImportLuaStub();

        LuaStubDefinition CurrentLuaStub { get; }

        bool HasLuaStub { get; }
    }
}
