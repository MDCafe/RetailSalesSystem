SELECT * FROM RMS.Products;

select p.Id as 'ProductId',p.Name as 'ProductName',pd.Price as 'Price',
pd.SellingPrice as 'SellingPrice',st.Quantity as 'Quantity', pd.PriceId as 'PriceId' 
from Products p, PriceDetails pd, Stocks st
where p.Id = pd.ProductId and pd.PriceId = st.PriceId
and st.Quantity !=0 
union
select p.Id as 'ProductId',p.Name as 'ProductName',pd.Price as 'Price',
pd.SellingPrice as 'SellingPrice',st.Quantity as 'Quantity', pd.PriceId as 'PriceId' 
from Products p, PriceDetails pd, Stocks st
where p.Id = pd.ProductId and pd.PriceId = st.PriceId
and st.Quantity = 0
and St.ModifiedOn = 
(select max(ModifiedOn) from Stocks s 
where s.ProductId = st.ProductId)
order by ProductName

group by p.id 
having count(*) <
order by p.Name,pd.PriceId desc


select max(rollingno) + 1 from category cat,customers c
where c.id = 3 and c.CustomerTypeId = cat.id

SELECT a.* FROM articles AS a
LEFT JOIN articles AS a2 
ON a.section = a2.section AND a.article_date <= a2.article_date
GROUP BY a.article_id
HAVING COUNT(*) <= 10;

SELECT * FROM RMS.Stocks;

insert into Stocks(ProductId, Quantity, PriceId)
select id,200,PriceId from products p, pricedetails pd
where p.id = pd.productid

delete from pricedetails where 1=1