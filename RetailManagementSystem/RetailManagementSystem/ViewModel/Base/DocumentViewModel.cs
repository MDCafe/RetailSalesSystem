using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

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

        #region CloseCommand
        /// <summary>
        /// This command closes a single file. The binding for this is in the AvalonDock LayoutPanel Style.
        /// </summary>
        abstract public ICommand CloseCommand
        {
            get;
        }

        //abstract public ICommand GetBillCommand
        //{
        //  get;
        //}
        #endregion

        #endregion properties

        public DocumentViewModel()
        {

        }
  }
}
