using System.Collections.Generic;
using Honours_Stage_Project.ViewModels;

namespace Honours_Stage_Project.Services
{
    public interface IImportService
    {
        (List<NodeViewModel>, List<Connection>) ImportDialogue(INodeConnectionService connectionService);

        LuaStubDefinition ImportLuaStub();
    }
}
