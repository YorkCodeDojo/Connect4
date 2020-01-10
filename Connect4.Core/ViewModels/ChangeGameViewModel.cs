using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Connect4.Core.ViewModels
{
    public class ChangeGameViewModel
    {
        public IEnumerable<SelectListItem> AllOtherPlayers { get; set; }
        public Guid? OtherPlayerID { get; set; }
        public string PlayerName { get; set; }
    }
}