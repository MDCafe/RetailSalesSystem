USE `rms`;
DROP procedure IF EXISTS `GetStockBalances`;

DELIMITER $$
USE `rms`$$
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetStockBalances`(in categoryId int,in productId int, in companyId int)
BEGIN 

if(categoryId = 0 && productId = 0 && companyId = 0)
then
    select p.Id as productId,p.Name,ReorderPoint,c.name CategoryName,cp.Name CompanyName,
	sum(s.quantity) Quantity
	from products p Join category c on c.Id = p.CategoryId
                    Join Stocks s on s.ProductId = p.Id
                    Join Companies cp on cp.id = p.CompanyId
	group by p.id;
					
elseif (categoryId !=0 && productId = 0 && companyId = 0)
then
 select p.Id as productId,p.Name,ReorderPoint,c.name CategoryName,cp.Name CompanyName,
	sum(s.quantity) Quantity
	from products p Join category c on c.Id = p.CategoryId and c.Id = categoryId
                    Join Stocks s on s.ProductId = p.Id
                    Join Companies cp on cp.id = p.CompanyId
	group by p.id;		
    
elseif (categoryId =0 && productId != 0 && companyId = 0)
then
	select p.Id as productId,p.Name,ReorderPoint,c.name CategoryName,cp.Name CompanyName,
	sum(s.quantity) Quantity
	from products p Join category c on c.Id = p.CategoryId 
                    Join Stocks s on s.ProductId = p.Id
                    Join Companies cp on cp.id = p.CompanyId
    where p.Id = productId                
	group by p.id;	
elseif (categoryId =0 && productId = 0 && companyId != 0)
then
	select p.Id as productId,p.Name,ReorderPoint,c.name CategoryName,cp.Name CompanyName,
	sum(s.quantity) Quantity
	from products p Join category c on c.Id = p.CategoryId 
                    Join Stocks s on s.ProductId = p.Id
                    Join Companies cp on cp.id = p.CompanyId and cp.Id = companyId            
	group by p.id;	
end if;    

END$$

DELIMITER ;

