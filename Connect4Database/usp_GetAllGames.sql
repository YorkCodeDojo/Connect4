CREATE PROCEDURE [dbo].[usp_GetAllGames] AS
BEGIN
	SET NOCOUNT ON

	SELECT G.GUID,
		   G.RedPlayerID,
		   G.State,
		   G.YellowPlayerID
	  FROM dbo.Game G

END