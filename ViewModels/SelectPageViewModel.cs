using Prism.Commands;
using Prism.Regions;
using Prism_HairStylishUp.Views;
using System.Windows.Controls;
using System.Windows.Shapes;
using Prism.Unity;
using static Prism_HairStylishUp.MainModel;
using Unity;

namespace Prism_HairStylishUp.ViewModels
{
    public class SelectPageViewModel : AddBindableBase,INavigationAware
    {
        /// <summary>
        /// 状態遷移マネージャー
        /// </summary>
        IRegionManager _regionManager;
        IUnityContainer _container;
        Camera camera;
        
        

        public SelectPageViewModel(IUnityContainer container, IRegionManager regionManager)
        {
            this._container = container;
            this._regionManager = regionManager;
            camera = new Camera();
            
            //if (!camera.ScreenCapture.IsOpened())
            //{
            //    Error.Visibility = System.Windows.Visibility.Visible;
            //}
        }

        

        private Button _cameraButton;
        public Button CameraButton
        {
            get{ return _cameraButton; }
            set{ SetProperty(ref _cameraButton, value);}
        }

        private Rectangle _error;
        public Rectangle Error
        {
            get { return _error; }
            set 
            { 
                SetProperty(ref _error, value);
            }

        }



        #region ボタンコマンド
        private DelegateCommand _libraryCommand;
        public DelegateCommand LibraryCommand
        {
            get { return _libraryCommand ?? (_libraryCommand = new DelegateCommand(ToLibrary)); }
        }

        private DelegateCommand _cameraCommand;
        public DelegateCommand CameraCommand
        {
            get { return _cameraCommand ?? (_cameraCommand = new DelegateCommand(ToCamera)); }
        }
        #endregion

        #region ボタンロジック
        private void ToLibrary()
        {
            ///Version1.1変更
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(LibraryPage), p);
   
        }

        private bool CanToLibrary()
        {
            bool b = !this.HasErrors;

            return b;
        }

        private void ToCamera()
        {
            ///Version1.1変更
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(CameraPage), p);
        }

        /// <summary>
        /// CanExecute
        /// </summary>
        /// <returns>bool</returns>
        //private bool camerabutton_canexecute()
        //{
        //    bool b = !camera.screencapture.isopened();
            
        //    return b;
        //}

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //camera.CameraOn();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //throw new System.NotImplementedException();
        }
        #endregion
    }
}