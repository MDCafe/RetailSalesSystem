USE `rms`;
DROP procedure IF EXISTS `GetSales`;

DELIMITER $$
USE `rms`$$
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetSales`(IN fromSalesDate Date, IN toSalesDate date,IN categoryId int,in internalUserId int)
BEGIN
select s.BillId,s.AddedOn,C.Name as Customer,CustomerOrderNo,
CASE
        WHEN s.TransportCharges= 0.00 THEN null
        ELSE s.TransportCharges
    END AS 'TransportCharges',
	CASE
        WHEN s.isCancelled =1 THEN 'Cancelled'
        ELSE NULL
    END AS 'Cancelled',
RunningBillNo,s.addedOn,
sum(sd.Discount) + if(isnull(s.discount),0,s.discount) Discount,
(sum(sd.sellingprice *sd.qty) - (sum(if(isnull(sd.discount),0,sd.discount)) + if(isnull(s.discount),0,s.discount))) + if(isnull(s.TransportCharges),0,s.TransportCharges) TotalAmount,
CASE
        WHEN s.PaymentMode = '0' AND (isnull(s.AmountPaid) = 1 OR s.AmountPaid = 0) THEN 
					(sum(sd.sellingprice *sd.qty) - (sum(if(isnull(sd.discount),0,sd.discount)) + if(isnull(s.discount),0,s.discount)))
					+ if(isnull(s.TransportCharges),0,s.TransportCharges)
        ELSE 
			s.AmountPaid
    END AS 'Cash Sales',
    CASE
        WHEN PaymentMode = '1'  THEN 
				(sum(sd.sellingprice *sd.qty) - (sum(if(isnull(sd.discount),0,sd.discount)) + if(isnull(s.discount),0,s.discount))) 
                -(if(isnull(s.AmountPaid),0,s.AmountPaid)) + if(isnull(s.TransportCharges),0,s.TransportCharges) 
        ELSE NULL
    END AS 'Credit Sales'
from sales s,Customers c, SaleDetails sd
where s.CustomerId = c.Id
and sd.BillId = s.BillId
and c.CustomerTypeId = categoryId
and -- Date(s.addedOn) >= fromSalesDate and Date(s.addedOn) <= toSalesDate and
(s.RunningBillNo > 
	(select EndBillNo from DateBillMapping 
	where id = (select id from DateBillMapping where date(EndOfDate) < fromSalesDate and CustomerTypeId = categoryId order by date(EndOfDate) desc LIMIT 1 
	))
and 
s.RunningBillNo <= (select EndBillNo from DateBillMapping where date(EndOfDate) = toSalesDate and CustomerTypeId = categoryId)
)
and (if(isnull(s.IsCancelled),0,s.IsCancelled)) = 0 
and s.UpdatedBy = if(internalUserId = 0,s.UpdatedBy,internalUserId)
group by s.billId
order by s.RunningBillNo 


/*union

select ModifiedOn, (select Name as 'Name' from customers where Id = (select customerId from sales sa where sa.BillId =rs.billid)) as Customer,
'','','Return',(select RunningBillNo from sales sa where sa.BillId =rs.billid) RunningBillNo,rs.CreatedOn,'','','', 
-rs.Quantity * (select sellingPrice from PriceDetails where PriceId = rs.PriceId) TotalAmount
from ReturnDamagedStocks rs 
where Date(rs.ModifiedOn) >= fromSalesDate and Date(rs.ModifiedOn) <= toSalesDate*/ ;
END$$

DELIMITER ;

