using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Windows.Media.Imaging;
using static Prism_HairStylishUp.MainModel;

namespace Prism_HairStylishUp.ViewModels
{
    public class ResultPageViewModel : BindableBase, INavigationAware
    {
        MainModel M;

        #region プロパティ
        private Protocol _protocol;
        public Protocol Protocol
        {
            get { return _protocol; }
            set { SetProperty(ref _protocol, value);}
        }

        private Encode _encode;
        public Encode Encode
        {
            get { return _encode; }
            set { SetProperty(ref _encode, value);}
        }

        private string _getData;
        public string GetData
        {
            get { return _getData; }
            set { SetProperty(ref _getData, value);}
        }

        private string _convertedB64;
        public string ConvertedB64
        {
            get{ return _convertedB64; }
            set{ SetProperty(ref _convertedB64, value); }
        }

        private string _flg;
        public string Flg
        {
            get{ return _flg; }
            set{ SetProperty(ref _flg, value); }
        }

        private BitmapImage _result;
        public BitmapImage Result
        {
            get{ return _result; }
            set{ SetProperty(ref _result, value); }
        }

        private WriteableBitmap _capturedPicture;
        public WriteableBitmap CapturedPicture
        {
            get{ return _capturedPicture; }
            set{ SetProperty(ref _capturedPicture, value); }
        }

        #endregion 

        IDialogService _dialogService;

        private readonly IRegionManager _regionManager;

        public ResultPageViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            M = new MainModel();

            _regionManager = regionManager;
            _dialogService = dialogService;
        }

        #region ボタンコマンド
        //キャンセルボタン
        private DelegateCommand repeatcapturecommand;
        public DelegateCommand RepeatCaptureCommand
        {
            get { return repeatcapturecommand ?? (repeatcapturecommand = new DelegateCommand(this.RepeatCapture)); }
        }

        //結果ボタン
        private DelegateCommand resultcapturecommand;

        public DelegateCommand ResultCaptureCommand
        {
            get { return resultcapturecommand ?? (resultcapturecommand = new DelegateCommand(this.Print)); }
        }
        #endregion

        private void RepeatCapture()
        {
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(Views.SelectPage), p);
        }

        //GETリクエスト処理
        private void Print()
        {     
            //MessageBox.Show("保存されました。");
            var message = new DialogParameters();
            //message.Add(nameof(MessageBoxViewModel)
            var filedirectory = App.Directory + "\\" + "result.bmp";

            Result = Encode.Base64toImage(ConvertedB64, filedirectory);
            message.Add(nameof(MessageBoxViewModel.Message), App.Directory + "に保存しました。");
            _dialogService.ShowDialog(nameof(MessageBox), message, null);
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(Views.SelectPage), p);
        }

        /// <summary>Viewを表示した後呼び出されます。</summary>
        /// <param name="navigationContext">Navigation Requestの情報を表すNavigationContext。</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ConvertedB64 = navigationContext.Parameters.GetValue<string>(nameof(ConvertedB64));
            Result = navigationContext.Parameters.GetValue<BitmapImage>(nameof(Result));
            CapturedPicture = navigationContext.Parameters.GetValue<WriteableBitmap>(nameof(CapturedPicture));
            Protocol = navigationContext.Parameters.GetValue<Protocol>(nameof(Protocol));
            Encode = navigationContext.Parameters.GetValue<Encode>(nameof(Encode));
            Flg = navigationContext.Parameters.GetValue<string>(nameof(Flg));
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
            Result = null;
            ConvertedB64 = null;
            CapturedPicture = null;
        }
    }
}
