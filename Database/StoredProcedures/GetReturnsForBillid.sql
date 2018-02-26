CREATE DEFINER=`RMS`@`%` PROCEDURE `GetReturnsForBillid`(IN runningBillNo integer, IN category integer)
BEGIN

declare billidValue int;

SET  billidValue = GetbillIdForCustomers(runningBillNo,category);

select pr.*,p.Name,cm.Description,(pr.ReturnPrice * pr.Quantity) ReturnAmount  
from purchasereturn pr, products p,PriceDetails pd, CodeMaster cm
where pr.productid = p.id
and pd.PriceId = pr.PriceId
and cm.code = "RTN" and pr.billId = billidValue
and cm.Id = pr.ReturnReasonCode;


END