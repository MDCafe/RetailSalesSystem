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
where Date(s.addedOn) between fromSalesDate and toSalesDate;
END