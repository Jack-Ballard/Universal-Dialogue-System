using System.Collections.Generic;
using Honours_Stage_Project.ViewModels;

namespace Honours_Stage_Project.Services
{
    public interface IImportService
    {
        (List<NodeViewModel>, List<Connection>) Import(INodeConnectionService connectionService, string fileName = "exported_data");
    }
}
