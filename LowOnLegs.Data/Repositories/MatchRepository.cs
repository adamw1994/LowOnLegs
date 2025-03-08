using LowOnLegs.Core.DTOs;
using LowOnLegs.Core.Models;
using LowOnLegs.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowOnLegs.Data.Repositories
{
    class MatchRepository: IMatchRepository
    {
        async public Task<bool> Add(Match match)
        {
            using (var context = new ApplicationDbContext())
            {
                await context.Matches.AddAsync(match);
            }
            return true;
        }
    }
}
