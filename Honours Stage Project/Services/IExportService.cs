using System.Collections.Generic;
using Honours_Stage_Project.ViewModels;

namespace Honours_Stage_Project.Services
{
    public interface IExportService
    {
        void Export(IEnumerable<NodeViewModel> nodes, IEnumerable<Connection> connections, string fileName = "exported_data");
    }
}
