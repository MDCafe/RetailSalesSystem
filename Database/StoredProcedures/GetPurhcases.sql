DELIMITER $$
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetPurchases`(IN runningBillNo integer, IN category integer)
BEGIN

declare billidValue int;

select billId into billidValue from purchases p where p.RunningBillNo = runningBillNo and
p.CompanyId in (select Id from companies c where c.CategoryTypeId = Category);

select p.addedon,p.BillId,p.RunningBillNo,
(select name from Companies where id = p.CompanyId) Supplier,
InvoiceNo,p.Discount,SpecialDiscount,TotalBillAmount,
IsCancelled,
(Select name from Products where id = ProductId) Product,
(select Price from PriceDetails where PriceId = pd.PriceId) Price,
(select freeqty from PurchaseFreeDetails pfd where pfd.billid = p.BillId and pfd.ProductId = pd.ProductId) FreeIssue,
pd.PurchasedQty,
pd.ActualPrice,
pd.Discount ItemDiscount,
p.cooliecharges cooliecharges,
p.kcooliecharges KcoolieCharges,
p.transportcharges trannsportcharges,
p.localCoolieCharges localCoolieCharges
from 
purchases p, purchasedetails pd
where p.BillId = pd.BillId
and p.BillId = billidValue;
END$$
DELIMITER ;
