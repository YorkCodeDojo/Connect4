using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Connect4.Controllers
{
    public class HomeController : Controller
    {

        private readonly Database database;
        public HomeController()
        {
            this.database = new Database();
        }

        public async Task<ActionResult> Index()
        {
            // Display a list of all subscribed players
            var allPlayers = await this.database.GetAllPlayers();

            return View(allPlayers);
        }
    }
}
