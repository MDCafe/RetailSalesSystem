CREATE FUNCTION `GetbillId` (runningBillNo integer,category integer)
RETURNS INTEGER
BEGIN

declare billidValue int;

select billId into billidValue from purchases p where p.RunningBillNo = runningBillNo and
p.CompanyId in (select Id from companies c where c.CategoryTypeId = Category);

RETURN billidValue;
END
