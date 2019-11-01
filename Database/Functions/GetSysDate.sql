use rms;
drop FUNCTION if exists `GetSysDate` ;

DELIMITER $$
CREATE DEFINER=`RMS`@`%` FUNCTION `GetSysDate`() RETURNS datetime DETERMINISTIC
BEGIN

declare sysDateTime datetime;

SELECT 
    sysdate()
INTO sysDateTime FROM
    dual;

RETURN sysDateTime;
END$$
DELIMITER ;
