USE `rms`;
DROP procedure IF EXISTS `GetProductsToOrderForProductIds`;

DELIMITER $$
USE `rms`$$
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetProductsToOrderForProductIds`(in productsIn varchar(255))
BEGIN
    SELECT 
    p.Name,
    SUM(st.Quantity) 'Available Qty',
    c.name 'CategoryName',
    p.ReorderPoint
FROM
    stocks st,
    Products p,
    Category c 
WHERE
	FIND_IN_SET(p.id, productsIn)
    AND st.ProductId = p.id
    AND p.CategoryId = c.Id
    AND p.IsActive = 1
GROUP BY c.id , st.ProductId , p.ReorderPoint
HAVING SUM(st.Quantity) <= p.ReorderPoint
ORDER BY c.name , p.Name;
END$$

DELIMITER ;

