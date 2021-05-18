using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace Prism_HairStylishUp
{
    public class MessageBoxViewModel : BindableBase, IDialogAware
    {
        public MessageBoxViewModel()
        {
            OKButton = new DelegateCommand(OKButtonExecute);
        }

        public string Title => "QRコード";

        public event Action<IDialogResult> RequestClose;

        private string _message = string.Empty;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public DelegateCommand OKButton {get;}
        

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            //throw new NotImplementedException();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            //throw new NotImplementedException();
            Message = parameters.GetValue<string>(nameof(Message));
            //ConvertedtoQR = parameters.GetValue<BitmapImage>(nameof());
        }

        private void OKButtonExecute()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }
    }
}
