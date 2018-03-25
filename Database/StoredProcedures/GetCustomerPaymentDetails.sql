USE `rms`;
DROP procedure IF EXISTS `GetCustomerPaymentDetails`;

DELIMITER $$
USE `rms`$$
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetCustomerPaymentDetails`(in customerId int)
BEGIN
SELECT 
    s.billid,
    s.RunningBillNo,
    SUM(pd.AmountPaid) AmountPaid,
    (s.TotalAmount - SUM(pd.AmountPaid)) BalanceAmount,
    s.TotalAmount,
    s.AddedOn,
    s.PaymentMode
FROM
    rms.PaymentDetails pd,
    sales s
WHERE
    s.BillId = pd.BillId
        AND s.customerId = pd.customerid
        AND pd.customerId = customerId
        AND IFNULL(s.IsCancelled, 0) = 0
        AND s.PaymentMode = 1
GROUP BY s.RunningBillNo
HAVING AmountPaid != s.TotalAmount
ORDER BY s.RunningBillNo;
END$$

DELIMITER ;

