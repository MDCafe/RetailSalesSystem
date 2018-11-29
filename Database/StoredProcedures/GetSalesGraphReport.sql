USE `RMS`;
DROP procedure IF EXISTS `GetSalesGraphReport`;

DELIMITER $$
USE `RMS`$$
CREATE DEFINER=`RMS`@`%` PROCEDURE `GetSalesGraphReport`()
BEGIN
DECLARE CreditSalesCursor CURSOR FOR
	(SELECT SUM(totalamount) TotalAmount, DATE_FORMAT(s.addedOn, '%b-%y') SalesYearMonth
	FROM sales s,Customers c
	WHERE  paymentMode = 1
    and s.CustomerId = c.id 
	and c.CustomerTypeId !=7 
	GROUP BY YEAR(s.AddedOn) , MONTH(s.AddedOn));

DECLARE HotelSalesCursor CURSOR FOR
	(SELECT SUM(totalamount) TotalAmount, DATE_FORMAT(s.addedOn, '%b-%y') SalesYearMonth
	FROM sales s,Customers c
	WHERE  paymentMode = 1
    and s.CustomerId = c.id 
	and c.CustomerTypeId =7 
	GROUP BY YEAR(s.AddedOn) , MONTH(s.AddedOn));

Drop table IF exists `SalesReportTbl`;

Create temporary table SalesReportTbl
(SELECT SUM(totalamount) CashSales, DATE_FORMAT(addedOn, '%b-%y') SalesYearMonth,0 CreditSales, 0 HotelSales,0 TotalSales
	FROM sales
	WHERE  paymentMode = 0
	GROUP BY YEAR(AddedOn) , MONTH(AddedOn)
);

open CreditSalesCursor;
Begin
		DECLARE exit_flag INT DEFAULT 0;			
		DECLARE amt decimal;
		Declare yearMonth varchar(20); 
		DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET exit_flag = 1;
        
get_creditLoop: LOOP
	FETCH CreditSalesCursor INTO amt,yearMonth;
		IF exit_flag THEN 
			LEAVE get_creditLoop;
		END IF;
		update SalesReportTbl set CreditSales = amt where SalesYearMonth = yearMonth;	
	END LOOP get_creditLoop;
End;
close CreditSalesCursor;

open HotelSalesCursor;
Begin
		DECLARE exit_flag INT DEFAULT 0;			
		DECLARE hotelAmt decimal;
		Declare hotelYearMonth varchar(20); 
	    DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET exit_flag = 1;

get_hotelsLoop: LOOP
	FETCH HotelSalesCursor INTO hotelAmt,hotelYearMonth;
		IF exit_flag THEN 
			LEAVE get_hotelsLoop;
		END IF;
		update SalesReportTbl set HotelSales = hotelAmt where SalesYearMonth = hotelYearMonth;	
	END LOOP get_hotelsLoop;
End;
Close  HotelSalesCursor;   
            
update SalesReportTbl stb set TotalSales = stb.CashSales + stb.CreditSales + stb.HotelSales;

select * from SalesReportTbl;

END$$

DELIMITER ;

