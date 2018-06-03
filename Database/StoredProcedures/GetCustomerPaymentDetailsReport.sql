USE `rms`;
DROP procedure IF EXISTS `GetCustomerPaymentDetailsReport`;

DELIMITER $$
USE `rms`$$
CREATE PROCEDURE `GetCustomerPaymentDetailsReport` (in fromDate datetime,in toDate datetime,in customerId int)
BEGIN
SELECT 
    s.RunningBillNo,
    pd.BillId,
    s.AddedOn BillDate,
    s.TotalAmount,
    pd.AmountPaid,
    (s.TotalAmount - pd.AmountPaid) BalanceAmount,
    pd.PaymentDate PaymentDate,
	/*case when cm.Id = 7 then 'Ca'
		when  cm.Id = 9 then 'Ch' end 'Description',*/
    cpd.ChequeNo,
    cpd.ChequeDate,
    case when ifnull(IsChequeRealised,0) != 0 and cpd.IsChequeRealised = 1 then 'Y' end 'Chq.Realised'
FROM
    rms.PaymentDetails pd  Join sales s on s.BillId = pd.BillId AND s.customerId = pd.customerid
						   left Join CodeMaster cm on pd.PaymentMode = cm.Id 
						   left join ChequePaymentDetails cpd on cpd.PaymentId = pd.Id
WHERE
        pd.customerId = customerId
        AND IFNULL(s.IsCancelled, 0) = 0
        AND date(s.AddedOn) >= fromDate
        AND date(s.AddedOn) <= toDate
/*GROUP BY s.RunningBillNo*/
ORDER BY s.RunningBillNo;
END$$

DELIMITER ;