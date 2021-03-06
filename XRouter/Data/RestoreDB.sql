--params
DECLARE
  @from  nvarchar(50) = 'XRouter',
  @newDB nvarchar(50) = 'XRouter' --XRouter, XRouterTestABC...


--fill just once
DECLARE
  --from where
  @addressFrom nvarchar(200) = 'C:\Program Files\Microsoft SQL Server\MSSQL10.DOMA\MSSQL\Backup\',
  --to where
  @SQLInstanceAddress nvarchar(200) = 'C:\Program Files\Microsoft SQL Server\MSSQL10.DOMA\MSSQL'
 
  
DECLARE
  @Disk nvarchar(200) = @addressFrom + @from + '.bak',
  @DataAddress nvarchar(200) = @SQLInstanceAddress + '\DATA\' + @newDB + '.mdf',
  @FromLog nvarchar(60) = @from + '_log',
  @LogAddress nvarchar(200) = @SQLInstanceAddress + '\DATA\' + @newDB + '_log.ldf'
  
RESTORE DATABASE @newDB
FROM  DISK = @disk WITH FILE = 1,
MOVE @from TO @DataAddress,
MOVE @FromLog TO @LogAddress,
NOUNLOAD,  REPLACE,  STATS = 10

GO

/*
sqlcmd usage:
make sure you have remote connections allowed in the SQL Server instance!

fill @from, @newDB in the declaration with $(from), $(newDB)

call the script like:
sqlcmd -E -i RestoreDB.sql -v from="pokusDB" newDB="pokusDB3"

test it, then you may create a .bat file...
*/
