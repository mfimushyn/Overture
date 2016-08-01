SELECT *
FROM his.Country 

SELECT *
FROM his.Track 


--DBCC CHECKIDENT ('his.Track' , RESEED)

SELECT *
FROM his.Race R 
	INNER JOIN his.GrandPrix GP
	ON GP.GrandPrixId = R.GrandPrixId 
	INNER JOIN his.Country C
	ON C.CountryId = GP.CountryId 

SELECT *
FROM his.Race 

SELECT *
FROM his.GrandPrix 

SELECT @@VERSION 


