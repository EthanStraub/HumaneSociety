DELETE FROM Animals
DBCC CHECKIDENT ('HumaneSociety.dbo.Animals',RESEED, 0)