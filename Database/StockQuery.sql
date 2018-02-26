select p.Id as productId,p.Code oldcode,p.Name,ReorderPoint,c.id categoryId,c.name,pd.PriceId, pd.Price,pd.SellingPrice,p.CompanyId,cp.Name,
COALESCE(st.ClosingBalance,s.Quantity) Quantity
	from products p Join category c on c.Id = p.CategoryId
					Join Companies cp on cp.Id = p.CompanyId
					Join PriceDetails pd on pd.ProductId = p.Id
                    Join Stocks s on s.ProductId = p.Id and s.PriceId = pd.PriceId
					left Join StockTransaction st on st.StockId = s.Id and date(st.addedOn) = '2017-11-21'		
    
   