CREATE DEFINER=`RMS`@`%` PROCEDURE `GetCurrentStock`(in categoryId int,in productId int, in companyId int)
BEGIN

if(categoryId = 0 && productId = 0 && companyId = 0)
then
	select p.Id as productId,p.Code oldcode,p.Name,ReorderPoint,c.id,c.name, pd.Price,pd.SellingPrice,p.CompanyId,cp.Name
	from products p , category c, pricedetails pd,companies cp
	where p.CategoryId = c.Id
	and p.CompanyId = cp.Id
	and pd.ProductId = p.id;
elseif (categoryId !=0 && productId = 0 && companyId = 0)
then
	select p.Id as productId,p.Code oldcode,p.Name,ReorderPoint,c.id,c.name, pd.Price,pd.SellingPrice,p.CompanyId,cp.Name
	from products p , category c, pricedetails pd,companies cp
	where p.CategoryId = c.Id
	and p.CompanyId = cp.Id
	and pd.ProductId = p.id
    and c.Id = categoryId;
    
elseif (categoryId =0 && productId != 0 && companyId = 0)
then
		select p.Id as productId,p.Code oldcode,p.Name,ReorderPoint,c.id,c.name, pd.Price,pd.SellingPrice,p.CompanyId,cp.Name
	from products p , category c, pricedetails pd,companies cp
	where p.CategoryId = c.Id
	and p.CompanyId = cp.Id
	and pd.ProductId = p.id
    and p.id = productId;
    
elseif (categoryId =0 && productId = 0 && companyId != 0)
then
		select p.Id as productId,p.Code oldcode,p.Name,ReorderPoint,c.id,c.name, pd.Price,pd.SellingPrice,p.CompanyId,cp.Name
	from products p , category c, pricedetails pd,companies cp
	where p.CategoryId = c.Id
	and p.CompanyId = cp.Id
	and pd.ProductId = p.id
    and cp.id = companyId;
end if;    

END