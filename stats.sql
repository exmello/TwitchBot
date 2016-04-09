select count(streamid) cnt, username
from viewers v
inner join Streams s on v.StreamId = s.Id
where s.Channel = 'itskatnip'
group by Username
having count(streamid) > 4
order by cnt desc

select sum(DATEDIFF(hour, v.joined, v.LastSeen)) totalhours, username
from viewers v
inner join Streams s on v.StreamId = s.Id
where s.Channel = 'itskatnip'
group by Username
order by totalhours desc