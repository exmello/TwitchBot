delete from TopBibleNames where word like '%.%'
delete from TopBibleNames where word like '%''%'
delete from TopBibleNames where word like '%?%'
delete from TopBibleNames where word like '%:%'
delete from TopBibleNames where word like '%;%'
delete from TopBibleNames where len(word) < 3
delete from TopBibleNames where word in ('let','and','the','then','for','but','now','this','who'
,'what','when','Therefore','That', 'There','yea','wherefore','all','thy','How','Take','why','Yet'
,'with','even','hear','shall','from','surely','woe','she','speak','after','every','fear','have'
,'Which','praise','howbeit','also','our','nor','Whosoever','art','are','say','lest','though','see'
,'put','high','did','turn','will','only','tell','test','hast','mine','while','according','out','sing','come','their'
,'lot','know','again','upon','cursed','her','get','return','here','before','can','set','red','not','these'
,'keep','salute','whoso','Hearken','Deliver','nay','until','peace','send','hath','look','thine','thus','one','most')
delete from TopBibleNames where total <= 22



select * from TopBibleNames t
inner join Nouns n on t.word = n.word
select * from TopBibleNames order by total desc