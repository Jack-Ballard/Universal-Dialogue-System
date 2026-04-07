using System.Collections.Generic;
using Honours_Stage_Project.ViewModels;

namespace Honours_Stage_Project.Services
{
    public interface IImportService
    {
        (List<NodeViewModel>, List<(int, int, int, int)>) Import(INodeConnectionService connectionService, string fileName = "exported_data");
    }
}
