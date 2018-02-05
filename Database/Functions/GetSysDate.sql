DELIMITER $$
CREATE DEFINER=`RMS`@`%` FUNCTION `GetSysDate`(runningBillNo integer,category integer) RETURNS datetime
BEGIN

declare sysDateTime datetime;

SELECT 
    sysdate()
INTO sysDateTime FROM
    dual;

RETURN sysDateTime;
END$$
DELIMITER ;
