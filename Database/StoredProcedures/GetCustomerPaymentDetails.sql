USE `rms`;
DROP procedure IF EXISTS `GetCustomerPaymentDetails`;

DELIMITER $$
USE `rms`$$
CREATE PROCEDURE `GetCustomerPaymentDetails` (in customerId int)
BEGIN
select s.billid,s.RunningBillNo,pd.AmountPaid,s.TotalAmount,s.IsCancelled,s.AddedOn,s.PaymentMode 
from rms.PaymentDetails pd, sales s
where s.BillId = pd.BillId
and s.customerId = pd.customerid
and pd.customerId = customerId;

END$$

DELIMITER ;

