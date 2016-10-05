CREATE PROCEDURE [dbo].[usp_LoadGame](@ID AS UNIQUEIDENTIFIER) AS
BEGIN
	SET NOCOUNT ON

	SELECT G.GUID,
		   G.RedPlayerID,
		   G.State,
		   G.YellowPlayerID
	  FROM dbo.Game G
	 WHERE G.GUID = @ID

END