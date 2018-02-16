CREATE DEFINER=`RMS`@`%` PROCEDURE `GetCustomerWiseSales`(in fromDate DateTime, in toDate Datetime,in customerId int)
BEGIN
select s.*,c.Name from sales s,customers c
where s.customerId = c.id
and c.id = customerId
and Date(s.addedOn) >= fromDate and Date(s.addedOn) <= toDate;
END