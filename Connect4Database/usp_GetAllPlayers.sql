CREATE PROCEDURE [dbo].[usp_GetAllPlayers] AS
BEGIN
	SET NOCOUNT ON

	SELECT P.GUID AS ID,
		   P.TeamName,		   
		   MAX(G.GUID) AS CurrentGameID,
		   P.SystemBot,
		   P.Password
	  FROM dbo.Players P
 LEFT JOIN dbo.Game G ON G.RedPlayerID = P.GUID OR G.YellowPlayerID = P.GUID
  GROUP BY P.GUID, P.TeamName, P.SystemBot, P.Password
  ORDER BY P.TeamName
END