using System.Threading.Tasks;
using Connect4.Core.Models;

namespace Connect4.Core.Bots
{
    public interface IBot
    {
        Task MakeMove(Game game);
    }
}