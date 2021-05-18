using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Prism_HairStylishUp.Views;
using static Prism_HairStylishUp.MainModel;
using Unity;

namespace Prism_HairStylishUp.ViewModels
{
    public class CameraPageViewModel : AddBindableBase, INavigationAware
    {
        bool isTask = true;
        Camera camera;
        Protocol protocol;
        Encode encode;

        private readonly IRegionManager _regionManager;
        IUnityContainer _container;

        #region プロパティ

        //Base64の文字列
        public string BASE64String { get; set; }

#if DEBUG
        //Getプロトコル文字列
        private string _get_data;
        public string Get_Data
        {
            get { return _get_data; }
            set { SetProperty(ref _get_data, value); }
        }
#else
#endif

        private string _flg = "0";
        public string Flg
        {
            get { return _flg; }
            set { SetProperty(ref _flg, value); }
        }

        //Postプロトコル文字列
        private int _post_data;
        public int Post_Data
        {
            get { return _post_data; }
            set { SetProperty(ref _post_data, value); }
        }

        //カメラで映されている画像のプロパティ
        private WriteableBitmap _bmp;
        public WriteableBitmap Bmp
        {
            get { return _bmp; }
            set { SetProperty(ref _bmp, value); }
        }

        //撮影した画像を保存するプロパティ
        private WriteableBitmap _picture;
        public WriteableBitmap Picture
        {
            get { return _picture; }
            set { SetProperty(ref _picture, value); }
        }

        //変換された画像を保存するプロパティ
        private BitmapImage _convert_bmp;
        public BitmapImage Convert_Bmp
        {
            get { return _convert_bmp; }
            set { SetProperty(ref _convert_bmp, value); }
        }

        //トーグルプロパティ
        private bool _toggle = true;
        public bool Toggle
        {
            get { return _toggle; }
            set
            {
                SetProperty(ref _toggle, value);
                if (_toggle == true)
                {
                    ConvertLabel = "短く";
                    Flg = "0";
                }
                else
                {
                    ConvertLabel = "長く";
                    Flg = "1";
                }
            }
        }

        private string _convertlabel = "短く";
        public string ConvertLabel
        {
            get { return _convertlabel; }
            set { SetProperty(ref _convertlabel, value); }
        }
        #endregion

        #region ボタンコマンド
        //撮影
        private DelegateCommand takepicturecommand;
        public DelegateCommand TakePictureCommand
        {
            get { return takepicturecommand ?? (takepicturecommand = new DelegateCommand(TakePicture)); }
        }

        //POSTリクエストボタン
        private DelegateCommand _postCommand;
        public DelegateCommand PostCommand
        {
            get { return _postCommand ?? (_postCommand = new DelegateCommand(this.PostRequest)); }
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new DelegateCommand(this.Cancel)); }
        }

#if DEBUG
     //GETリクエストボタン
        private DelegateCommand getrequestcommand;
        public DelegateCommand GetRequestCommand
        {
            get { return getrequestcommand ?? (getrequestcommand = new DelegateCommand(this.GetRequest)); }
        }

        //DEBUGボタン
        private DelegateCommand debugcommand;

        public DelegateCommand DebugCommand
        {
            get { return debugcommand ?? (debugcommand = new DelegateCommand(Debug_Code)); }
        }

        public string Title => "写真ページ";
#else


#endif
        #endregion

        #region コンストラクタ
        public CameraPageViewModel(IRegionManager regionManager, IUnityContainer container)
        {
            //_dialogService = dialogService;
            this._regionManager = regionManager;
            this._container = container;
            protocol = new Protocol();
            encode = new Encode();
        }
#endregion

        #region ボタンコマンドロジック

        //カメラONの処理
        private async void StartCapture()　//非同期処理 async await
        {
            isTask = true;
            camera = new Camera();
            camera.CameraOn();
            await ShowImage();
        }

        //カメラOFFの処理
        private void StopCapture()
        {
            isTask = false;
            Bmp = null;
            camera.Dispose();
        }

        private void Cancel()
        {
            ///Version1.1変更
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(Views.SelectPage), p);
            Picture = null;
            BASE64String = null;
            Convert_Bmp = null;
        }

        //撮影処理
        private void TakePicture()
        {
            BASE64String = camera.TakePicture();
            this.Picture = camera.Picture;
            Post_Data = camera.Width;
        }

        //POSTリクエスト処理
        private void PostRequest()
        {
            TakePicture();

            protocol.PostRequest(BASE64String, Flg);
            Convert_Bmp = protocol.pro_Convert_Bmp;


            ///<param name="p">結果ページにデータを伝達</param>
            ///Version1.1変更
            var p = new NavigationParameters();
            p.Add(nameof(ResultPageViewModel.Result), Convert_Bmp);
            p.Add(nameof(ResultPageViewModel.ConvertedB64), BASE64String);
            p.Add(nameof(ResultPageViewModel.Protocol), protocol);
            p.Add(nameof(ResultPageViewModel.Encode), encode);
            p.Add(nameof(ResultPageViewModel.Flg), _flg);
            p.Add(nameof(ResultPageViewModel.CapturedPicture), Picture);

#if DEBUG

            Get_Data = protocol.pro_Get_Data;
            p.Add(nameof(ResultPageViewModel.GetData), Get_Data);
#else
            
#endif
            _regionManager.RequestNavigate("ContentRegion", nameof(ResultPage), p);
            
        }

#if DEBUG
        private void GetRequest() { }
        private void Debug_Code() { }

#else
            
#endif

        #endregion

        #region タスクの速度
        /// Task
        /// 30秒Delayで画面を表示(30fps?)
        private async Task ShowImage()
        {
            while (isTask)
            {
                Bmp = camera.Capture();
                if (Bmp == null) break;

                await Task.Delay(30);
            }
        }
#endregion

        /// <summary>Viewを表示した後呼び出されます。</summary>
        /// <param name="navigationContext">Navigation Requestの情報を表すNavigationContext。</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            StartCapture();
        }

        /// <summary>表示するViewを判別します。</summary>
        /// <param name="navigationContext">Navigation Requestの情報を表すNavigationContext。</param>
        /// <returns>表示するViewかどうかを表すbool。</returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <summary>別のViewに切り替わる前に呼び出されます。</summary>
        /// <param name="navigationContext">Navigation Requestの情報を表すNavigationContext。</param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            StopCapture();
        }
    }
}

