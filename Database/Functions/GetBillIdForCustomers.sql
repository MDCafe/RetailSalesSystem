use rms;
drop function if exists `GetbillIdForCustomers` ;
DELIMITER $$
CREATE DEFINER=`RMS`@`%` FUNCTION `GetbillIdForCustomers`(runningBillNo integer,category integer) RETURNS int DETERMINISTIC
BEGIN

declare billidValue int;

SELECT 
    billId
INTO billidValue FROM
    purchases p
WHERE
    p.RunningBillNo = runningBillNo
        AND p.CompanyId IN (SELECT 
            Id
        FROM
            CUSTOMERS c
        WHERE
            c.CustomerTypeId = Category);

RETURN billidValue;
END$$
DELIMITER ;
