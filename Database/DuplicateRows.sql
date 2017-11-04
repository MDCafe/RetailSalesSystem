use RMS;(1469,1475,1618)

SELECT * FROM PriceDetails	WHERE ProductId in
(
SELECT ProductId
FROM PriceDetails
GROUP BY ProductId, Price
HAVING count(*) > 1)