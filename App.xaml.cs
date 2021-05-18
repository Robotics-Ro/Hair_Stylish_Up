using Prism_HairStylishUp.Views;
using Prism_HairStylishUp.ViewModels;
using Prism.Ioc;
using System.Windows;

namespace Prism_HairStylishUp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static string _localfilepath = System.AppDomain.CurrentDomain.BaseDirectory;
        public static string LocalFilePath { get { return _localfilepath; } }
        static string image_folder = "\\image";
        
        private static string _folderPath = App.LocalFilePath + image_folder;
        public static string Directory { get { return _folderPath; } }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MessageBox, MessageBoxViewModel>();
            containerRegistry.RegisterForNavigation<SelectPage, SelectPageViewModel>();
            containerRegistry.RegisterForNavigation<CameraPage, CameraPageViewModel>();
            containerRegistry.RegisterForNavigation<LibraryPage, LibraryPageViewModel>();
            containerRegistry.RegisterForNavigation<ResultPage, ResultPageViewModel>();
        }

    }
}
