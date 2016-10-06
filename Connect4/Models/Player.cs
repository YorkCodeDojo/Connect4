using System;

namespace Connect4.Models
{
    public class Player
    {

        public Player(string teamName)
        {
            this.Name = teamName;
        }

        public Player(string teamName, Guid playerID)
        {
            this.Name = teamName;
            this.ID = playerID;
        }

        public Guid? CurrentGameID { get; set; }
        public Guid ID { get; set; }
        public string Name { get; set; }

        public bool SystemBot { get; set; }
    }
}