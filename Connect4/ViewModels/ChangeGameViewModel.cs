using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Connect4.Models;

namespace Connect4.ViewModels
{
    public class ChangeGameViewModel
    {
        public IEnumerable<SelectListItem> AllOtherPlayers { get; set; }
        public Guid? OtherPlayerID { get; set; }
        public string PlayerName { get; set; }
    }
}