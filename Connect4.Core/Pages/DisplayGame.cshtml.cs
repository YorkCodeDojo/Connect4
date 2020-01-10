using System;
using System.Threading.Tasks;
using Connect4.Core.Models;
using Connect4.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Connect4.Core
{
    public class DisplayGameModel : PageModel
    {
        private readonly Database _database;

        public Game ViewModel { get; set; } = new Game();

        public DisplayGameModel(Database database)
        {
            _database = database;
        }
        public async Task<ActionResult> OnGet(Guid? gameID)
        {
            if (!gameID.HasValue) return LocalRedirect("/Index");

            var game = await _database.LoadGame(gameID.Value);
            if (game == null) return LocalRedirect("/Index");

            ViewData.Add("RedPlayer", await GetPlayerName(game.RedPlayerID));
            ViewData.Add("YellowPlayer", await GetPlayerName(game.YellowPlayerID));

            ViewModel = game;
            
            return Page();
        }

        private async Task<string> GetPlayerName(Guid playerID)
        {
            var player = await _database.LoadPlayer(playerID);
            if (player is null) return "Unknown";
            return player.Name;
        }
    }
}