using System.Collections.Generic;
using Honours_Stage_Project.ViewModels;

namespace Honours_Stage_Project.Services
{
    public interface IExportService
    {
        void Export(IEnumerable<object> nodeExports, IEnumerable<Connection> connections, string fileName = "exported_data");
    }
}
