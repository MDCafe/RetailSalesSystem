USE `RMS`;
DROP procedure IF EXISTS `MarkEndOfDay`;

DELIMITER $$
USE `RMS`$$
CREATE DEFINER=`RMS`@`%` PROCEDURE `MarkEndOfDay`()
BEGIN
declare systemDateTime datetime; 
declare maxRunningBillNo int; 

set systemDateTime = (select GetSysDate()); 

select ifnull(max(s.RunningBillNo),0)  into maxRunningBillNo
from sales s
where date(s.AddedOn) = date(systemDateTime) and
s.CustomerId in (select Id from customers c where c.CustomerTypeId = 7);

call MarkEndOfDayOnCustomerType(systemDateTime, maxRunningBillNo,7);

select ifnull(max(s.RunningBillNo),0)  into maxRunningBillNo
from sales s
where date(s.AddedOn) = date(systemDateTime) and
s.CustomerId in (select Id from customers c where c.CustomerTypeId != 7);

call MarkEndOfDayOnCustomerType(systemDateTime, maxRunningBillNo,9);
 
END$$

DELIMITER ;

