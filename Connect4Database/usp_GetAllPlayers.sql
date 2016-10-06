CREATE PROCEDURE [dbo].[usp_GetAllPlayers] AS
BEGIN
	SET NOCOUNT ON

	SELECT P.GUID AS ID,
		   P.TeamName,		   
		   G.GUID AS CurrentGameID,
		   P.SystemBot
	  FROM dbo.Players P
 LEFT JOIN dbo.Game G ON G.RedPlayerID = P.GUID OR G.YellowPlayerID = P.GUID
  ORDER BY P.TeamName
END