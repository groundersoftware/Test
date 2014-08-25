IF EXISTS (select * from [dbo].[Configuration])
BEGIN
   UPDATE [dbo].[Configuration] SET [Version] = 1
END
ELSE
   INSERT INTO [dbo].[Configuration] VALUES (1)
