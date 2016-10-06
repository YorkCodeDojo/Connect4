using System.Threading.Tasks;
using System.Web.Http;
using Connect4.Models;

namespace Connect4.Controllers.API
{
    /// <summary>
    /// Contains methods to registry new players
    /// </summary>
    public class RegisterController : APIControllerBase
    {
        /// <summary>
        /// Call this to register your team name.  This call returns your PlayerID which you will need to supply in all other calls
        /// </summary>
        /// <param name="teamName">The unique name of your team</param>
        /// <param name="password">A password to stop cheating!</param>
        [HttpPost]
        public async Task<IHttpActionResult> POST(string teamName, string password)
        {
            // Generate a unique playerID
            var player = new Player(teamName);
            await database.SavePlayer(player, password);

            // Reload the player
            player = await database.LoadPlayer(player.ID);
            if (player == null) return BadRequest("No player with this ID exists");

            // Create them a game for them to develop against
            if (!player.CurrentGameID.HasValue)
            {
                await CreateInitialGame(player.ID);
            }
            return Ok(player.ID);
        }

    }
}
