create database JSONDb
go
use JSONDb
go
create table jsonTab(data json)
go



CREATE PROCEDURE jsonsp
    @jsonData JSON
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO jsonTab (data) values (@jsonData)
END;

