using Prism.Ioc;
using Prism.Unity;
using System.Windows;
using SqlImport.PopUp;
using SqlImport.PopUpViewModels;
using SqlImport.Views;

namespace SqlImport
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<Register, RegisterViewModel>("Register");

            containerRegistry.RegisterDialog<SelectFile, SelectFileViewModel>("SelectFile");

            containerRegistry.RegisterDialog<PopUp.Task, TaskViewModel>("Task");
        }
    }
}
