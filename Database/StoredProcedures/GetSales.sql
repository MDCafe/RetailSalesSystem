CREATE DEFINER=`RMS`@`%` PROCEDURE `GetSales`(IN fromSalesDate Date, IN toSalesDate date)
BEGIN
select AddedOn,(select Name as 'Name' from customers where id = s.customerId) as Customer,CustomerOrderNo,TransportCharges,
	CASE
        WHEN s.isCancelled =1 THEN 'Cancelled'
        ELSE NULL
    END AS 'Cancelled',
RunningBillNo,s.addedOn,
Discount,
CASE
        WHEN s.PaymentMode = '0' THEN TotalAmount
        ELSE NULL
    END AS 'Cash Sales',
    CASE
        WHEN PaymentMode = '1'  THEN TotalAmount
        ELSE NULL
    END AS 'Credit Sales',
TotalAmount

from sales s
where Date(s.addedOn) between fromSalesDate and toSalesDate

union

select ModifiedOn, (select Name as 'Name' from customers where Id = (select customerId from sales sa where sa.BillId =rs.billid)) as Customer,
'','','Return',(select RunningBillNo from sales sa where sa.BillId =rs.billid) RunningBillNo,rs.CreatedOn,'','','', 
-rs.Quantity * (select sellingPrice from PriceDetails where PriceId = rs.PriceId) TotalAmount
from ReturnDamagedStocks rs 
where rs.ModifiedOn between fromSalesDate and toSalesDate;
END