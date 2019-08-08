USE `rms`;
DROP procedure IF EXISTS `GetSalesDetailsForBillId`;

DELIMITER $$
USE `rms`$$
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetSalesDetailsForBillId`(IN runningBillNo integer, IN category integer)
BEGIN
declare billidValue int;
SET  billidValue = GetbillIdForCustomers(runningBillNo,category);
SELECT 
    s.AddedOn,    
    c.Name Customer,
    CustomerOrderNo,
    TransportCharges,
    RunningBillNo,
    s.Discount,
    CASE
        WHEN s.PaymentMode = '0' THEN 'Cash'
        ELSE 'Credit'
    END AS PaymentMode,
    TotalAmount,
    sd.Discount AS ItemDiscount,
    price AS Price,
    p.Name AS ProductName,
    sd.Qty,
    sd.SellingPrice,sd.productId
FROM
    sales s inner join SaleDetails sd on (s.BillId = sd.BillId and s.billId = billidValue)
			inner Join Customers c on c.id = s.customerId
            inner join pricedetails pd on (sd.PriceId = pd.PriceId )
            inner join Products p on (sd.ProductId = p.Id)
order by sd.id;        
END$$

DELIMITER ;

