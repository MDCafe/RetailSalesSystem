using log4net;
using RetailManagementSystem.Command;
using RetailManagementSystem.Utilities;
using RetailManagementSystem.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Expenses
{
    internal class ExpenseEntryViewModel : DocumentViewModel
    {
        public List<CodeMaster> ExpenseTypesList { get; private set; }
        public DateTime TranscationDate { get; set; }
        public ObservableCollection<ExpenseDetail> ExpenseDetailList { get; set; }
        public decimal TotalAmount { get; set; }

        static readonly ILog _log = LogManager.GetLogger(typeof(ExpenseEntryViewModel));

        public ExpenseEntryViewModel()
        {
            using (RMSEntities rmsEntities = new RMSEntities())
            {
                ExpenseTypesList = rmsEntities.CodeMasters.Where(c => c.Code == "EXP").ToList();
            }
            TranscationDate = RMSEntitiesHelper.Instance.GetSystemDBDate();
            ExpenseDetailList = new ObservableCollection<ExpenseDetail>();

            ExpenseDetailList.CollectionChanged += OnExpenseDetailsListCollectionChanged;
            Title = "Expense Details";
        }

        #region SaveCommand
        RelayCommand<object> _saveCommand = null;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand<object>(async (p) => await OnSave(p), (p) =>
                    {
                        return ExpenseDetailList.Count != 0 && ExpenseDetailList[0].ExpenseTypeId != 0;
                    });
                }

                return _saveCommand;
            }
        }


        private async Task OnSave(object parameter)
        {
            //short paramValue = Convert.ToInt16(parameter);

            ////Validate for Errors
            if (!Validate()) return;
            //if (_isEditMode)
            //{
            //    SaveOnEdit(paramValue);
            //    return;
            //}

            PanelLoading = true;

            try
            {
                var salesSaveTask = Task.Factory.StartNew(() =>
                {
                    using (var rmsEntitiesSaveCtx = new RMSEntities())
                    {
                        RemoveProductWithNullValues();
                        try
                        {
                            foreach (var item in ExpenseDetailList)
                            {
                                item.AddedOn = GetCombinedDateTime();
                            }
                            rmsEntitiesSaveCtx.ExpenseDetails.AddRange(ExpenseDetailList);
                            rmsEntitiesSaveCtx.SaveChanges();
                            Clear();
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Error while saving..!!", ex);
                            throw ex;
                        }
                    }
                }).ContinueWith(
                    (t) =>
                    {
                        PanelLoading = false;
                        //if (!_autoResetEvent.SafeWaitHandle.IsClosed)
                        //{
                        //    _autoResetEvent.Set();
                        //}
                    });
                await salesSaveTask;
            }
            catch (Exception ex)
            {
                //if (paramValue == SaveOperations.SaveOnWindowClosing) return;
                Utility.ShowErrorBox("Error while saving..!!" + ex.Message);
            }
            finally
            {
                PanelLoading = false;
                //if (!_autoResetEvent.SafeWaitHandle.IsClosed)
                //{
                //    _autoResetEvent.Set();
                //}
            }
        }

        internal override void Clear()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                ExpenseDetailList.Clear();
            });
            TranscationDate = RMSEntitiesHelper.GetServerDate();
        }

        private DateTime GetCombinedDateTime()
        {
            DateTime combinedDateTime;
            //Get the current time since it takes the window open time
            DateTime date = TranscationDate.Date;
            TimeSpan time = RMSEntitiesHelper.GetServerTime();
            combinedDateTime = date.Add(time);
            return combinedDateTime;
        }


        private bool Validate()
        {
            foreach (var expItem in ExpenseDetailList)
            {
                if (expItem.ExpenseTypeId == 0) continue;

                if (!expItem.Amount.HasValue)
                {
                    Utility.ShowErrorBox("Amount can't be null");
                    return false;
                }

                return true;
            }
            return true;
        }




        //private void SaveOnEdit(short parameter)
        //{
        //    PanelLoading = true;
        //    var salesSaveOnEditTask = System.Threading.Tasks.Task.Run(() =>
        //    {
        //        using (var rmsEntities = new RMSEntities())
        //        {
        //            try
        //            {

        //                Clear();
        //            }
        //            catch (Exception ex)
        //            {
        //                Utility.ShowErrorBox("Error while saving on edit..!!" + ex.Message);
        //                _log.Error("Error while saving on edit..!!", ex);
        //                CloseCommand.Execute(null);
        //            }
        //        }
        //    }).ContinueWith(
        //    (t) =>
        //    {
        //        PanelLoading = false;
        //        CloseCommand.Execute(null);
        //    });
        //}

        //private void RemoveDeletedItems(RMSEntities rmsEntities)
        //{
        //    foreach (var saleDetailExtn in _deletedItems)
        //    {
        //        //New item added and removed
        //        if (saleDetailExtn.BillId == 0) continue;
        //        var saleDetail = rmsEntities.SaleDetails.FirstOrDefault(s => s.BillId == saleDetailExtn.BillId && s.ProductId == saleDetailExtn.ProductId);

        //        var stockNewItem = rmsEntities.Stocks.FirstOrDefault(s => s.ProductId == saleDetail.ProductId && s.PriceId == saleDetail.PriceId);
        //        if (stockNewItem != null)
        //        {
        //            var qty = saleDetail.Qty.Value;
        //            var stockTrans = rmsEntities.StockTransactions.Where(s => s.StockId == stockNewItem.Id).OrderByDescending(s => s.AddedOn).FirstOrDefault();

        //            stockTrans.Outward -= qty;
        //            stockTrans.ClosingBalance += qty;

        //            stockNewItem.Quantity += qty;
        //        }
        //        rmsEntities.SaleDetails.Remove(saleDetail);
        //    }
        //}

        private void RemoveProductWithNullValues()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                foreach (var item in ExpenseDetailList.Reverse())
                {
                    if (item.ExpenseTypeId == 0)
                        ExpenseDetailList.Remove(item);
                }
            });
        }

        private void OnExpenseDetailsListCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                if (!(e.OldItems[0] is ExpenseDetail ExpenseDetailItem) || ExpenseDetailItem.ExpenseTypeId <= 0) return;
                //if (_isEditMode)
                //  _deletedItems.Add(SaleDetailExtn);
                TotalAmount -= ExpenseDetailItem.Amount ?? ExpenseDetailItem.Amount.Value;
                RaisePropertyChanged("TotalAmount");
            }
        }
        #endregion

    }
}
