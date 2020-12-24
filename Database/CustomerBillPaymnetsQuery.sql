

select * from paymentdetails pd
where BillId in(
select s.BillId from paymentdetails pd, sales s
where pd.CustomerId = 171
and s.BillId = pd.BillId and 
s.RunningBillNo in(3840,4307));

