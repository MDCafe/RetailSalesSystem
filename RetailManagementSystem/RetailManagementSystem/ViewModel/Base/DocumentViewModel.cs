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
    private bool mIsFilePathReal = false;
    #endregion Fields

    #region properties
    

    //abstract public string FilePath { get; protected set; }

    abstract public bool IsDirty { get; set; }

    #region CloseCommand
    /// <summary>
    /// This command cloases a single file. The binding for this is in the AvalonDock LayoutPanel Style.
    /// </summary>
    abstract public ICommand CloseCommand
    {
        get;
    }

        abstract public ICommand SaveCommand
    {
      get;
    }
    #endregion
    #endregion properties
  }
}
