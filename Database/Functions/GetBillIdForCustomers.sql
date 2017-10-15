DELIMITER $$
CREATE DEFINER=`RMS`@`%` FUNCTION `GetbillIdForCustomers`(runningBillNo integer,category integer) RETURNS int(11)
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
