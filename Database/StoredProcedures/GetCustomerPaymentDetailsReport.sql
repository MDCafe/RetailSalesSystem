USE `rms`;
DROP procedure IF EXISTS `GetCustomerPaymentDetailsReport`;

DELIMITER $$
USE `rms`$$
CREATE PROCEDURE `GetCustomerPaymentDetailsReport` (in customerId int)
BEGIN
SELECT 
    s.RunningBillNo,
    s.AddedOn BillDate,
    s.TotalAmount,
    pd.AmountPaid,
    (s.TotalAmount - SUM(pd.AmountPaid)) BalanceAmount,
	cm.Description,
    cpd.ChequeNo,
    cpd.ChequeDate,
    case when cpd.IsChequeRealised != null and cpd.IsChequeRealised = 1 then 'Realised' end 'Chq.Realised'
FROM
    rms.PaymentDetails pd  Join sales s on s.BillId = pd.BillId AND s.customerId = pd.customerid
						   left Join CodeMaster cm on pd.PaymentMode = cm.Id 
						   left join ChequePaymentDetails cpd on cpd.PaymentId = pd.Id
WHERE
        pd.customerId = customerId
        AND IFNULL(s.IsCancelled, 0) = 0
GROUP BY s.RunningBillNo
ORDER BY s.RunningBillNo;
END$$

DELIMITER ;

