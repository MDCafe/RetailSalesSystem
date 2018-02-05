CREATE DEFINER=`RMS`@`%` PROCEDURE `GetPurchaseReturnForCompany`(IN filterCompanyId int )
BEGIN

select pr.* from purchaseReturn pr, products p
where pr.ProductId = p.Id
and p.CompanyId = filterCompanyId
and pr.MarkedForReturn = true
and pr.ReturnReasonCode != 3;

END