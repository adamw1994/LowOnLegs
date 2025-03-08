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
    class PlayerRepository: IPlayerRepository
    {
        public PlayerDto Get(int playerId)
        {
            using (var context = new ApplicationDbContext())
            {
                var player = context.Players.FirstOrDefault(p => p.PlayerId == playerId);
                return new PlayerDto(player);
            }
        }

        public IEnumerable<PlayerDto> GetPlayers()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Players.Select(p => new PlayerDto(p)).ToList();
            }
        }
    }
}
