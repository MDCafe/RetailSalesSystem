USE `rms`;
DROP procedure IF EXISTS `GetCustomerPaymentDetails`;

DELIMITER $$
USE `rms`$$
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetCustomerPaymentDetails`(in customerId int)
BEGIN
select s.billid,s.RunningBillNo,sum(pd.AmountPaid) AmountPaid,s.TotalAmount,s.AddedOn,s.PaymentMode 
from rms.PaymentDetails pd, sales s
where s.BillId = pd.BillId
and s.customerId = pd.customerid
and pd.customerId = customerId
and ifnull(s.IsCancelled,0) = 0
and s.PaymentMode = 1
group by s.RunningBillNo
having AmountPaid != s.TotalAmount
order by s.RunningBillNo;
END$$

DELIMITER ;

