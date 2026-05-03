using System.Windows;
using Honours_Stage_Project.Services;
using Honours_Stage_Project.ViewModels;

namespace Honours_Stage_Project
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var fileService = new FileService();
            var importService = new JsonImportService(fileService);
            var exportService = new JsonExportService(fileService, importService);
            var luaValidationService = new LuaStubValidationService(importService);
            var connectionService = new NodeConnectionService();
            var dialogService = new DialogService();

            var viewModel = new MainWindowViewModel(
                connectionService,
                exportService,
                importService,
                luaValidationService,
                dialogService);

            new MainWindow(viewModel).Show();
        }
    }
}
