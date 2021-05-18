using Prism.Commands;
using Prism.Regions;
using System.IO;

namespace Prism_HairStylishUp.ViewModels
{
    public class MainWindowViewModel
    {

        /// <summary>
        /// 状態遷移マネージャー
        /// </summary>
        private readonly IRegionManager _regionManager;

        #region プロパティ
        private string Title => "#HairStyle_Up";
        #endregion

        #region ボタンコマンド
        private DelegateCommand _startCommand;
        public DelegateCommand StartCommand
        {
            get { return _startCommand ?? (_startCommand = new DelegateCommand(Begin)); }
        }
        #endregion

        #region コンストラクタ
        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        #endregion

        #region ボタンコマンドロジック
        private void Begin()
        {
            DirectoryInfo di = new DirectoryInfo(App.Directory);

            if (!di.Exists)
            {
                di.Create();   
            }


            ///次のページに状態遷移
            ///Version1.1変更
            var p = new NavigationParameters();
            _regionManager.RequestNavigate("ContentRegion", nameof(Views.SelectPage), p);
        }
        #endregion
    }
}
