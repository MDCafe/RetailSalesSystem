select 
p.id,p.Name, sum((sd.SellingPrice - sd.CostPrice) * sd.qty) profit,
-- ((sum(sd.SellingPrice) - sum(sd.CostPrice))/sum(sd.SellingPrice)) * 100 percentage,
sum((sd.SellingPrice - sd.CostPrice) * sd.qty)/sum(sd.SellingPrice * sd.qty) * 100 percentage,
sd.qty Quantity
-- sum(sd.SellingPrice* sd.qty) SellingPrice,
-- sum(sd.CostPrice * sd.qty) CostPrice,
-- sum(sd.SellingPrice* sd.qty) - sum(sd.CostPrice * sd.qty) profit1
from saledetails sd
join products p on (sd.ProductId = p.id)
where date(sd.AddedOn) >= "2020-09-01" and date(sd.AddedOn) <= "2020-09-1"
and isnull(p.IsActive) = 1 or p.IsActive = 1
-- and sd.ProductId in(1124)
group by p.id
having isnull(profit) = 0
-- limit 100;