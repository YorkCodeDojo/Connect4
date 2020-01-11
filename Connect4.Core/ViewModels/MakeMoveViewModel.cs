using System;

namespace Connect4.Core.ViewModels
{
    public class MakeMoveViewModel
    {
        public Guid PlayerId { get; set; }

        public string Password { get; set; }

        public int ColumnNumber { get; set; }
    }
}
