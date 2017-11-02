CREATE DEFINER=`RMS`@`%` PROCEDURE `GetPurchases`(IN runningBillNo integer, IN category integer)
BEGIN

declare billidValue int;

SET  billidValue = GetbillIdForCompanies(runningBillNo,category);  

SELECT 
    p.addedon,
    p.BillId,
    p.RunningBillNo,
    (SELECT 
            name
        FROM
            Companies
        WHERE
            id = p.CompanyId) Supplier,
    InvoiceNo,
    p.Discount,
    SpecialDiscount,
    TotalBillAmount,
    IsCancelled,
    (SELECT 
            name
        FROM
            Products
        WHERE
            id = ProductId) Product,
    (SELECT 
            Price
        FROM
            PriceDetails
        WHERE
            PriceId = pd.PriceId) Price,
    (SELECT 
            freeqty
        FROM
            PurchaseFreeDetails pfd
        WHERE
            pfd.billid = p.BillId
                AND pfd.ProductId = pd.ProductId) FreeIssue,
    pd.PurchasedQty,
    pd.ActualPrice,
    pd.Discount ItemDiscount,
    p.cooliecharges cooliecharges,
    p.kcooliecharges KcoolieCharges,
    p.transportcharges trannsportcharges,
    p.localCoolieCharges localCoolieCharges
FROM
    purchases p,
    purchasedetails pd
WHERE
    p.BillId = pd.BillId
        AND p.BillId = billidValue;
        order by pd.Id asc
END