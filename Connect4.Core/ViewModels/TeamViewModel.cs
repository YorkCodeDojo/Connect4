using System;

namespace Connect4.Core.ViewModels
{
    public class TeamViewModel
    {
        public string Name { get; set; }
        public string Playing { get; set; }

        public string State { get; set; }

        public Guid? CurrentGameID { get; set; }
    }
}