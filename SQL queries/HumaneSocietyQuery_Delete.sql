DELETE FROM Employees
DBCC CHECKIDENT ('HumaneSociety.dbo.Employees',RESEED, 0)