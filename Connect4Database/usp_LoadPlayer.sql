CREATE PROCEDURE [dbo].[usp_LoadPlayer](@ID AS UNIQUEIDENTIFIER) AS
BEGIN

	SELECT P.GUID, 
		   P.TeamName,
		   G.GUID AS CurrentGameID,
		   P.SystemBot,
		   P.Password
	  FROM dbo.Players P
 LEFT JOIN dbo.Game G ON G.RedPlayerID = @ID OR G.YellowPlayerID = @ID
	 WHERE P.GUID = @ID

END