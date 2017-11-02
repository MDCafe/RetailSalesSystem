DELIMITER $$
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetProductPriceForPriceId`(IN priceId integer)
BEGIN

SELECT 
    p.Id AS 'ProductId',
    p.Name AS 'ProductName',
    pd.Price AS 'Price',
    pd.SellingPrice AS 'SellingPrice',
    st.Quantity AS 'Quantity',
    pd.PriceId AS 'PriceId',
    DATE_FORMAT(st.ExpiryDate, '%d/%m/%Y') AS 'ExpiryDate'
FROM
    Products p,
    PriceDetails pd,
    Stocks st
WHERE
    p.Id = pd.ProductId
        AND pd.PriceId = st.PriceId
        AND pd.PriceId = priceId
        ORDER BY ProductName;
END$$
DELIMITER ;
