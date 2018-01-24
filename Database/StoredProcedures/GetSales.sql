DELIMITER $$
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetSales`(IN fromSalesDate Date, IN toSalesDate date,IN categoryId int)
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
(sum(sd.sellingprice *sd.qty) - sum(sd.Discount)) + s.TransportCharges TotalAmount,
CASE
        WHEN s.PaymentMode = '0' THEN (sum(sd.sellingprice *sd.qty) - sum(sd.Discount)) + s.TransportCharges
        ELSE NULL
    END AS 'Cash Sales',
    CASE
        WHEN PaymentMode = '1'  THEN (sum(sd.sellingprice *sd.qty) - sum(sd.Discount)) + s.TransportCharges 
        ELSE NULL
    END AS 'Credit Sales'
from sales s,Customers c, SaleDetails sd
where s.CustomerId = c.Id
and sd.BillId = s.BillId
and c.CustomerTypeId = categoryId
and Date(s.addedOn) >= fromSalesDate and Date(s.addedOn) <= toSalesDate
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
