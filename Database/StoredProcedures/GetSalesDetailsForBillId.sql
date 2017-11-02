CREATE DEFINER=`RMS`@`%` PROCEDURE `GetSalesDetailsForBillId`(IN runningBillNo integer, IN category integer)
BEGIN

declare billidValue int;

SET  billidValue = GetbillIdForCustomers(runningBillNo,category);

SELECT 
    s.AddedOn,
    (SELECT 
            Name AS 'Name'
        FROM
            customers
        WHERE
            id = s.customerId) AS Customer,
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
    (SELECT 
            Price
        FROM
            priceDetails pd
        WHERE
            pd.PriceId = sd.PriceId
                AND pd.ProductId = sd.ProductId) AS Price,
    (SELECT 
            p.Name
        FROM
            Products p
        WHERE
            p.Id = sd.ProductId) AS ProductName,
    sd.Qty,
    sd.SellingPrice,sd.productId
FROM
    sales s,
    SaleDetails sd
WHERE
    s.BillId = sd.BillId
        AND s.billId = billidValue
        order by sd.id;
        
END