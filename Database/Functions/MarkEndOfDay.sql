USE `RMS`;
DROP function IF EXISTS `MarkEndOfDay`;

DELIMITER $$
USE `RMS`$$
CREATE FUNCTION `MarkEndOfDay`() RETURNS int
    DETERMINISTIC
BEGIN
declare maxRunningBillNo int; 
declare systemDateTime datetime; 
declare recordCount int;

set systemDateTime = (select GetSysDate()); 
select ifnull(max(s.RunningBillNo),0)  into maxRunningBillNo
from sales s
where date(s.AddedOn) = date(systemDateTime) and
s.CustomerId in (select Id from customers c where c.CustomerTypeId = 7);

select count(*) into recordCount
from DateBillMapping d
where date(d.EndOfDate) = (select date(GetSysDate()));

if(recordCount > 0 ) then 
	return 1;
end if;
insert DateBillMapping (EndOfDate,EndBillNo) values (systemDateTime,maxRunningBillNo);
return 0;
END$$

DELIMITER ;

