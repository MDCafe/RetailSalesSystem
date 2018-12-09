USE `rms`;
DROP procedure IF EXISTS `GetCustomerBalance`;

DELIMITER $$
USE `rms`$$
CREATE PROCEDURE `GetCustomerBalance` (in fromDate datetime,in toDate datetime,in customerId int)
BEGIN
select sum(BalAmount) BalanceAmount from
(
	SELECT 
		s.TotalAmount - sum(pd.AmountPaid) BalAmount
	FROM
		rms.PaymentDetails pd  Join sales s on (s.BillId = pd.BillId AND s.customerId = pd.customerid AND s.PaymentMode!=0)
							   left join ChequePaymentDetails cpd on cpd.PaymentId = pd.Id
	WHERE
			pd.customerId = customerId
			AND IFNULL(s.IsCancelled, 0) = 0
			AND date(s.AddedOn) >= fromDate
			AND date(s.AddedOn) <= toDate
		
	GROUP BY s.RunningBillNo
	ORDER BY s.RunningBillNo
) as Bal;
END$$

DELIMITER ;