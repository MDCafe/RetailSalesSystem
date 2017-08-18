CREATE DEFINER=`RMS`@`%` PROCEDURE `GetSales`(IN salesDate Date)
BEGIN
select AddedOn,(select Name as 'NAme' from customers where id = s.customerId) as Customer,
RunningBillNo,s.addedOn ,
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
where Date(s.addedOn) = salesDate;
END