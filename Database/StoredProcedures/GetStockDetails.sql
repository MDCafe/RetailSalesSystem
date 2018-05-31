USE `rms`;
DROP procedure IF EXISTS `GetStockDetails`;

DELIMITER $$
USE `rms`$$
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetStockDetails`(in categoryId int,in productId int, in companyId int, 
													 in fromDate date, in ToDate date)
BEGIN

if(categoryId = 0 && productId = 0 && companyId = 0)
then
	select p.Id as productId,p.Name,c.id categoryId,c.name CategoryName,pd.PriceId, pd.Price,
    pd.SellingPrice,p.CompanyId,cp.Name CompanyName,
	COALESCE(st.ClosingBalance,0) Quantity,st.Inward,st.outward,st.addedOn,st.SalesPurchaseCancelQty
	from products p Join category c on c.Id = p.CategoryId
					Join Companies cp on cp.Id = p.CompanyId
					Join PriceDetails pd on pd.ProductId = p.Id
                    Join Stocks s on s.ProductId = p.Id and s.PriceId = pd.PriceId
					left Join StockTransaction st on st.StockId = s.Id and date(st.addedOn) between fromDate and toDate;
elseif (categoryId !=0 && productId = 0 && companyId = 0)
then
	select p.Id as productId,p.Name,c.id categoryId,c.name CategoryName,pd.PriceId, pd.Price,
    pd.SellingPrice,p.CompanyId,cp.Name CompanyName,
	COALESCE(st.ClosingBalance,0) Quantity,st.Inward,st.outward,st.addedOn,st.SalesPurchaseCancelQty
	from products p Join category c on c.Id = p.CategoryId and c.id = categoryId
					Join Companies cp on cp.Id = p.CompanyId
					Join PriceDetails pd on pd.ProductId = p.Id
                    Join Stocks s on s.ProductId = p.Id and s.PriceId = pd.PriceId
					left Join StockTransaction st on st.StockId = s.Id and date(st.addedOn) between fromDate and toDate;		
    
elseif (categoryId =0 && productId != 0 && companyId = 0)
then
		select p.Id as productId,p.Name,c.id categoryId,c.name CategoryName,pd.PriceId, pd.Price,
    pd.SellingPrice,p.CompanyId,cp.Name CompanyName,
	COALESCE(st.ClosingBalance,0) Quantity,st.Inward,st.outward,st.addedOn,st.SalesPurchaseCancelQty
	from products p Join category c on c.Id = p.CategoryId
					Join Companies cp on cp.Id = p.CompanyId
					Join PriceDetails pd on pd.ProductId = p.Id
                    Join Stocks s on s.ProductId = p.Id and s.PriceId = pd.PriceId
					left Join StockTransaction st on st.StockId = s.Id and date(st.addedOn) between fromDate and toDate
    where p.Id = productId;
elseif (categoryId =0 && productId = 0 && companyId != 0)
then
	select p.Id as productId,p.Name,c.id categoryId,c.name CategoryName,pd.PriceId, pd.Price,
    pd.SellingPrice,p.CompanyId,cp.Name CompanyName,
	COALESCE(st.ClosingBalance,0) Quantity,st.Inward,st.outward,st.addedOn,st.SalesPurchaseCancelQty
	from products p Join category c on c.Id = p.CategoryId
					Join Companies cp on cp.Id = p.CompanyId and cp.Id = companyId
					Join PriceDetails pd on pd.ProductId = p.Id
                    Join Stocks s on s.ProductId = p.Id and s.PriceId = pd.PriceId
					left Join StockTransaction st on st.StockId = s.Id and date(st.addedOn) between fromDate and toDate;
end if;    

END$$

DELIMITER ;

