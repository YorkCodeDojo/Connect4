
CREATE PROCEDURE [dbo].[usp_SavePlayer](@TeamName AS NVARCHAR(100), @Password AS NVARCHAR(100)) AS
BEGIN
	SET NOCOUNT ON

	DECLARE @ID AS UNIQUEIDENTIFIER
	DECLARE @ExistingPassword AS NVARCHAR(100)
	
	SELECT @ID = GUID,
		   @ExistingPassword = P.Password
	  FROM dbo.Players P
	 WHERE P.TeamName = @TeamName

	IF (@ID IS NULL)
	BEGIN
		-- Team does not currently exist
		SELECT @ID = newID()
		
		INSERT INTO dbo.Players ( GUID, TeamName, Password)
		SELECT @ID, @TeamName, @Password
		
	END

	SELECT @ID AS ID
END
