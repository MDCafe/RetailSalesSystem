CREATE DEFINER=`RMS`@`%` PROCEDURE `GetSales`(IN fromSalesDate Date, IN toSalesDate date,IN categoryId int)
BEGIN
select s.AddedOn,C.Name as Customer,CustomerOrderNo,TransportCharges,
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

from sales s,Customers c
where s.CustomerId = c.Id
and c.CustomerTypeId = categoryId
and Date(s.addedOn) >= fromSalesDate and Date(s.addedOn) <= toSalesDate

/*union

select ModifiedOn, (select Name as 'Name' from customers where Id = (select customerId from sales sa where sa.BillId =rs.billid)) as Customer,
'','','Return',(select RunningBillNo from sales sa where sa.BillId =rs.billid) RunningBillNo,rs.CreatedOn,'','','', 
-rs.Quantity * (select sellingPrice from PriceDetails where PriceId = rs.PriceId) TotalAmount
from ReturnDamagedStocks rs 
where Date(rs.ModifiedOn) >= fromSalesDate and Date(rs.ModifiedOn) <= toSalesDate*/ ;
END