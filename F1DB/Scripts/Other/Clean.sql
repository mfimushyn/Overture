﻿--DELETE his.Track
--DELETE his.Country
--DBCC CHECKIDENT('his.Country',RESEED,0)
--DBCC CHECKIDENT('his.Track',RESEED,0)