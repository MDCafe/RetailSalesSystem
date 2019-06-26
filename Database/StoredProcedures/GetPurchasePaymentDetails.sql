USE `rms`;
DROP procedure IF EXISTS `GetPurchasePaymentDetails`;

DELIMITER $$
USE `rms`$$
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetPurchasePaymentDetails`(in paramCompanyId int)
BEGIN
SELECT 
    p.billid,
    p.RunningBillNo,
    SUM(pd.AmountPaid) AmountPaid,
    (p.TotalBillAmount - SUM(pd.AmountPaid)) BalanceAmount,
    p.TotalBillAmount,
    p.AddedOn,
    p.PaymentMode
FROM
    rms.PurchasePaymentDetails pd,
    Purchases p
WHERE
    p.BillId = pd.PurchaseBillId
        AND p.companyId = pd.companyId
        AND p.companyId  = paramCompanyId
        AND IFNULL(p.IsCancelled, 0) = 0
        AND p.PaymentMode = 1
GROUP BY p.RunningBillNo
HAVING AmountPaid != p.TotalBillAmount
ORDER BY p.RunningBillNo;
END$$

DELIMITER ;

