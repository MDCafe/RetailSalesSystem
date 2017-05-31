SELECT * FROM RMS.PriceDetails;
SELECT * FROM RMS.stocks;

insert into PriceDetails (billid,ProductId,Price,sellingprice) (select 1, id,125,130 from products)
insert into stocks (ProductId,quantity,priceid) (select Productid,500,priceId from priceDetails)
