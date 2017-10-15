DELIMITER $$
CREATE DEFINER=`RMS`@`%` FUNCTION `GetbillIdForCompanies`(runningBillNo integer,category integer) RETURNS int(11)
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
            companies c
        WHERE
            c.CategoryTypeId = Category);

RETURN billidValue;
END$$
DELIMITER ;
