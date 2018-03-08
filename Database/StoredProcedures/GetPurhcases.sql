USE `rms`;
DROP procedure IF EXISTS `GetPurchases`;

DELIMITER $$
USE `rms`$$
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetPurchases`(IN runningBillNo integer, IN category integer)
BEGIN

declare billidValue int;

SET  billidValue = GetbillIdForCompanies(runningBillNo,category);  

SELECT 
    p.addedon,
    p.BillId,
    InvoiceNo,
    p.Discount,
    p.cooliecharges cooliecharges,
    p.kcooliecharges KcoolieCharges,
    p.transportcharges trannsportcharges,
    p.localCoolieCharges localCoolieCharges,
    p.SpecialDiscount,
    p.TotalBillAmount,
    p.IsCancelled,
    c.Name Supplier,
    c.CategoryTypeId,
    CASE
        WHEN c.CategoryTypeId = '11' THEN concat('C', p.RunningBillNo)
        ELSE p.RunningBillNo
    END AS 'RunningBillNo'
from Purchases p, companies c
where p.CompanyId = c.Id
and p.BillId = billidValue;
END$$

DELIMITER ;

