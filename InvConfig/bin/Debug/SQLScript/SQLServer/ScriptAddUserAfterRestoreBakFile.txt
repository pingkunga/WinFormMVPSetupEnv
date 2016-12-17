--ตรวจสอบ User ก่อนว่า Instane มี User นี้ หรือป่าว ? ถ้าไม่ก็สร้าง
IF  NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'invest')
                CREATE LOGIN [invest] WITH PASSWORD = 0x0100E1BCBAF63A22EDDC4FCEE2551E32A45C51363F0A9AD4D95F HASHED, SID = 0x1B16028920ADF147BA9744E7CAE21EBF, DEFAULT_DATABASE = [master], CHECK_POLICY = OFF, CHECK_EXPIRATION = OFF
GO
IF  NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'bonanza')
                CREATE LOGIN [bonanza] WITH PASSWORD = 0x01004D0ACBE8F6923518FBFCE9832985129E2ECB2A2B23DA99C3 HASHED, SID = 0x5AAEDC9C5626F345AD2052EE739879FE, DEFAULT_DATABASE = [master], CHECK_POLICY = OFF, CHECK_EXPIRATION = OFF
GO
--////////////////////////////////////////////////////////
--จัดการ user ของ DB
--เอาสิทธิ์การเป็นเจ้าของ Schema INVEST ไปให้ dbo ก่อนเพื่อป้องกันปัญหาลบ User ไม่ได้ เพราะเป็นเจ้าของ Schema
ALTER AUTHORIZATION ON SCHEMA::INVEST TO dbo;
--ลบ User ของ DB
DROP USER [invest];
DROP USER [bonanza];
--สร้าง User ใหม่
CREATE USER bonanza FOR LOGIN bonanza WITH DEFAULT_SCHEMA = INVEST;
CREATE USER invest FOR LOGIN invest WITH DEFAULT_SCHEMA = INVEST;
Go
--ใส่ Role ให้เป็น DB OWNER
EXEC sp_addrolemember 'db_owner', 'bonanza'
Go
EXEC sp_addrolemember 'db_owner', 'invest'
Go
--คืนสิทธิ์ให้เจ้าของ Schema Invest กลับไปให้ user invest
ALTER AUTHORIZATION ON SCHEMA::INVEST TO invest;