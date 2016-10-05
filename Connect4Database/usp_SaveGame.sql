CREATE PROCEDURE [dbo].[usp_SaveGame] (@ID AS UNIQUEIDENTIFIER,
								       @YellowPlayerID AS UNIQUEIDENTIFIER,
									   @RedPlayerID AS UNIQUEIDENTIFIER,
									   @State AS NVARCHAR(MAX)) AS
BEGIN
	SET NOCOUNT ON

	UPDATE G
	   SET G.State = @State  
	  FROM dbo.Game G
	 WHERE G.GUID = @ID

	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO dbo.Game ( GUID, RedPlayerID, YellowPlayerID, State)
		SELECT @ID, @RedPlayerID, @YellowPlayerID, @State
	END
END