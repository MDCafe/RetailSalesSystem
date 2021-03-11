USE `rms`;
DROP procedure IF EXISTS `GetPOSSales`;

DELIMITER $$
USE `rms`$$
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetPOSSales`(IN fromSalesDate Date, IN toSalesDate date,in internalUserId int)
BEGIN
select s.BillId,s.AddedOn,C.Name as Customer,	
RunningBillNo,s.addedOn,
sum(s.TotalAmount)  TotalAmount,
CASE
        WHEN s.PaymentMode = '0' AND (isnull(s.AmountPaid) = 1 OR s.AmountPaid = 0) THEN 
					sum(s.TotalAmount)
        ELSE 
			s.AmountPaid
    END AS 'Cash Sales',
    CASE
        WHEN PaymentMode = '1'  THEN 
				sum(s.TotalAmount)
        ELSE NULL
    END AS 'Credit Sales'
from sales s,Customers c, SaleDetails sd
where s.CustomerId = c.Id
and sd.BillId = s.BillId and
Date(s.addedOn) >= fromSalesDate and Date(s.addedOn) <= toSalesDate
and (if(isnull(s.IsCancelled),0,s.IsCancelled)) = 0 
and s.UpdatedBy = if(internalUserId = 0,s.UpdatedBy,internalUserId)
group by s.billId
order by s.RunningBillNo;
END$$

DELIMITER ;

