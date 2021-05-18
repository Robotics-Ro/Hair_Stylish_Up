using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Text;
using System.Windows.Media.Imaging;
using static Prism_HairStylishUp.MainModel;

namespace Prism_HairStylishUp.ViewModels
{
    public class LibraryPageViewModel : BindableBase, INavigationAware
    {

        //string image = @"C:\Users\sympo\Documents\RO13\";
        /// <summary>
        /// 状態遷移マネージャー
        /// </summary>
        IRegionManager _regionManager;
        

        Protocol protocol;
        Encode encode;
        Camera camera;

        #region プロパティ

        //Base64の文字列
        public string BASE64String { get; set; }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { SetProperty(ref _fileName, value); }
        }

        //Getプロトコル文字列
        private int _get_data;
        public int Get_Data
        {
            get { return _get_data; }
            set { SetProperty(ref _get_data, value); }
        }

        private string _flg = "0";
        public string Flg
        {
            get { return _flg; }
            set { SetProperty(ref _flg, value); }
        }

        //Postプロトコル文字列
        private string _post_data;
        public string Post_Data
        {
            get { return _post_data; }
            set { SetProperty(ref _post_data, value); }
        }

        //カメラで映されている画像のプロパティ
        public BitmapImage LoadedImage { get; set; }

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

        //トーグルコマンドラベル
        private string _convertlabel = "短く";
        public string ConvertLabel
        {
            get { return _convertlabel; }
            set { SetProperty(ref _convertlabel, value); }
        }
        #endregion

        #region ボタンコマンド

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

        //読み込みボタン
        private DelegateCommand _loadcommand;

        public DelegateCommand LoadCommand
        {
            get { return _loadcommand ?? (_loadcommand = new DelegateCommand(Load)); }
        }
        #endregion

        #region コンストラクタ
        public LibraryPageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            protocol = new Protocol();
            encode = new Encode();
            camera = new Camera();
        }
        #endregion

        #region ボタンコマンドロジック
        //撮影処理
        private void TakePicture()
        {
            var temp = encode.BitmapFromWriteableBitmap(Picture);
            BASE64String = encode.BytetoBase64(temp); //BASE64変換
        }

        //POSTリクエスト処理
        private void PostRequest()
        {
            protocol.PostRequest(BASE64String, Flg);
            Post_Data = protocol.pro_Post_Data;

            string savePath = App.LocalFilePath + "test.txt";

            System.IO.File.WriteAllText(savePath, Post_Data, Encoding.Default); 

            Convert_Bmp = protocol.pro_Convert_Bmp;

            ///<param name="p">結果ページにデータを伝達</param>
            ///Version1.1変更
            var p = new NavigationParameters();
            p.Add(nameof(ResultPageViewModel.Result), Convert_Bmp);
            p.Add(nameof(ResultPageViewModel.ConvertedB64), protocol.Json.Img);
            p.Add(nameof(ResultPageViewModel.Protocol), protocol);
            p.Add(nameof(ResultPageViewModel.Encode), encode);
            p.Add(nameof(ResultPageViewModel.Flg), _flg);
            p.Add(nameof(ResultPageViewModel.CapturedPicture), Picture);

            _regionManager.RequestNavigate("ContentRegion", nameof(Views.ResultPage), p);
        }

        private void Cancel()
        {
            ///Version1.1変更
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(Views.SelectPage),p);
        }

        //DEBUGボタン処理
        public void Load()
        {
            BASE64String = ShowFileOpenDialog();

            Picture = new WriteableBitmap(LoadedImage);
        }

        public string ShowFileOpenDialog()
        {
            // OpenFileDialogを生成 
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            // 基本ファイル拡張子及び基本ファイル拡張子のフィルターを設定する。
            dialog.DefaultExt = ".jpg";
            dialog.Filter = "(*.bmp, *.jpg) | *.bmp; *.jpg; | すべてのファイル(*.*) | *.* ";
            ;
            dialog.Multiselect = true;

            // ShowDialog メソッドを使ってOpenFileDialogを生成 
            Nullable<bool> result = dialog.ShowDialog();

            // 選択したFile Name呼び出して出力
            if (result == true)
            {
                FileName = dialog.FileName;

                LoadedImage = new BitmapImage(new Uri(FileName));

                //Image Image = Image.FromFile(FileName);
                //var temp = BytesFromImage(Image);
                var temp = camera.Image(FileName);
                BASE64String = encode.BytetoBase64(temp);
            }

            return BASE64String;
        }
        #endregion

        /// <summary>Viewを表示した後呼び出されます。</summary>
        /// <param name="navigationContext">Navigation Requestの情報を表すNavigationContext。</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {

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
            Picture = null;
            BASE64String = null;
            Convert_Bmp = null;
            LoadedImage = null;
        }
    }
}
