sqlcmd -S (local)\SQL2016 -i %~dp0\..\BVT\TransientFaultHandling.BVT\Scripts\CreateTransientFaultHandlingTestDatabase.sql
sqlcmd -S (local)\SQL2016 -i %~dp0\..\BVT\TransientFaultHandling.BVT\Scripts\CreateTransientFaultHandlingTestDatabaseObjects.sql