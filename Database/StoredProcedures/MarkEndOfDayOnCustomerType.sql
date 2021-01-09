USE `RMS`;
DROP procedure IF EXISTS `MarkEndOfDayOnCustomerType`;

DELIMITER $$
USE `RMS`$$
CREATE DEFINER=`RMS`@`%` PROCEDURE `MarkEndOfDayOnCustomerType`(in systemDateTime datetime,
 in maxRunningBillNo int,in customerTypeId int)
BEGIN
declare recordCount int;

select count(*) into recordCount
from DateBillMapping d 
where date(d.EndOfDate) = (select date(GetSysDate())) and d.CustomerTypeId = customerTypeId;

if(recordCount > 0 ) then 
	select 1;
else
	insert DateBillMapping (EndOfDate,EndBillNo,customerTypeId) values (systemDateTime,maxRunningBillNo,customerTypeId);
	select 0;
end if;
END$$

DELIMITER ;

