using System;
using System.IO;
using System.Windows.Input;
using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;

namespace RetailManagementSystem.ViewModel
{
    class SalesEntryViewModel : DocumentViewModel
    {
    
        public SalesEntryViewModel()
        {
          IsDirty = true;
          //Title = FileName;
        }

        //#region FilePath
        //private string _filePath = null;    
        //#endregion

        //public string FileName
        //{
        //  get
        //  {
        //    if (FilePath == null)
        //      return "Noname" + (IsDirty ? "*" : "");

        //    return System.IO.Path.GetFileName(FilePath) + (IsDirty ? "*" : "");
        //  }
        //}

        #region TextContent

        private string _textContent = string.Empty;
        public string TextContent
        {
          get { return _textContent; }
          set
          {
            if (_textContent != value)
            {
              _textContent = value;
              RaisePropertyChanged("TextContent");
              IsDirty = true;
            }
          }
        }

        #endregion

        #region IsDirty

        private bool _isDirty = false;
        override public bool IsDirty
        {
          get { return _isDirty; }
          set
          {
            if (_isDirty != value)
            {
              _isDirty = value;
              RaisePropertyChanged("IsDirty");
              RaisePropertyChanged("FileName");
            }
          }
        }

            #endregion

        #region SaveCommand
         RelayCommand _saveCommand = null;
            override public ICommand SaveCommand
        {
          get
          {
            if (_saveCommand == null)
            {
              _saveCommand = new RelayCommand((p) => OnSave(p), (p) => CanSave(p));
            }

            return _saveCommand;
          }
        }

        #region CloseCommand
        RelayCommand _closeCommand = null;
        override public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand((p) => OnClose(), (p) => CanClose());
                }

                return _closeCommand;
            }
        }

        private bool CanClose()
        {
            return true;
        }

        private void OnClose()
        {
            //Workspace.This.Close(this);
        }
        #endregion

        public bool CanSave(object parameter)
        {
          return IsDirty;
        }

        private void OnSave(object parameter)
        {
          //Workspace.This.Save(this, false);
        }

        #endregion

   
        private bool CanSaveAs(object parameter)
        {
          return IsDirty;
        }

        public override Uri IconSource
        {
          get
          {
            // This icon is visible in AvalonDock's Document Navigator window
            return new Uri("pack://application:,,,/Edi;component/Images/document.png", UriKind.RelativeOrAbsolute);
          }
        }

        public void SetFileName(string f)
        {
          //this._filePath = f;
        }
    }
}
