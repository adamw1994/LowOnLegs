using LowOnLegs.Core.Models;

namespace LowOnLegs.Data.Repositories.Interfaces
{
    public interface IMatchRepository
    {
        Task Add(Match match);
        IEnumerable<Match> GetAll();
    }
}
