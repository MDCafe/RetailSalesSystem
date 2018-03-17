using System.Windows.Input;
using RetailManagementSystem.Command;

namespace RetailManagementSystem.ViewModel.Base
{

    /// <summary>
    /// Base class that shares common properties, methods, and intefaces
    /// among viewmodels that represent documents in Edi
    /// (text file edits, Start Page, Prgram Settings).
    /// </summary>
    internal abstract class DocumentViewModel : PaneViewModel
  {
        #region Fields
        private string _windowTitle;
        bool _panelLoading;
        #endregion Fields

        #region properties


        public string WindowName
        {
            get
            {
                //if (_windowTitle == null)
                //    return "New Window" + (IsDirty ? "*" : "");
                return _windowTitle;            
            }
            set
            {
                _windowTitle = value;
            }
        }

        //abstract public bool IsDirty { get; set; }

        public bool PanelLoading
        {
            get
            {
                return _panelLoading;
            }
            set
            {
                _panelLoading = value;
                RaisePropertyChanged("PanelLoading");
            }
        }

        #region CloseCommand
        RelayCommand<object> _closeCommand = null;
        public
            ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand<object>((p) => OnClose(), (p) => CanClose());
                }

                return _closeCommand;
            }
        }

        protected virtual bool CanClose()
        {
            return true;
        }

        protected virtual void OnClose()
        {
            Workspace.This.Close(this);
        }

        #endregion

        #region Clear Command
        RelayCommand<object> _clearCommand = null;

        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new RelayCommand<object>((p) => Clear());
                }

                return _clearCommand;
            }
        }

        virtual internal void Clear()
        {

        }

        #endregion

        #endregion properties
        
  }
}
