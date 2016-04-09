select * into TopBibleNames from (
select count(v.Id) total, split.Item word
from Verses v
cross apply fnSplit(' ', v.[Text]) split
where ASCII(left(split.Item, 1)) between ASCII('A') and ASCII('Z')
group by split.Item
having count(v.Id) > 10
--order by total desc
) as a