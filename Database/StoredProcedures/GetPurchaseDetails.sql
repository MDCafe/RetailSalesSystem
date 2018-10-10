USE `rms`;
DROP procedure IF EXISTS `GetPurchaseDetails`;

DELIMITER $$
USE `rms`$$
CREATE PROCEDURE `GetPurchaseDetails` (IN runningBillNo integer, IN category integer)
BEGIN

declare billidValue int;

SET  billidValue = GetbillIdForCompanies(runningBillNo,category); 

SELECT 
    prod.Name,
    pr.Price,
	pfd.freeqty as FreeIssue,
    pr.SellingPrice,
    purchdet.PurchasedQty,
    purchdet.ActualPrice,
    purchdet.Discount ItemDiscount,
    purchdet.ItemCoolieCharges Coolie,
    purchdet.ItemTransportCharges Trans
FROM purchasedetails purchdet Join PriceDetails pr on purchdet.PriceId = pr.PriceId
							  Join Products prod on  purchdet.ProductId = prod.Id
                              left join PurchaseFreeDetails pfd on purchdet.BillId = pfd.BillId and pfd.ProductId = prod.Id
WHERE
	purchdet.BillId = billidValue
	order by purchdet.Id asc;
END$$

DELIMITER ;

