/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF NOT EXISTS ( SELECT NULL FROM dbo.Players WHERE TeamName = 'Random Bot')
BEGIN
	INSERT INTO dbo.Players (GUID, Password, SystemBot, TeamName)
	SELECT 'A05BF67C-2BBB-4243-BF18-FE60C52CF4F9','qwerty',1,'Random Bot'
END