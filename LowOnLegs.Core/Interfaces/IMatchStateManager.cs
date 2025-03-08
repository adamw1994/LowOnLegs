using LowOnLegs.Core.DTOs;
using LowOnLegs.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowOnLegs.Core.Interfaces
{
    public interface IMatchStateManager
    {
        MatchStateDto AddPoint(PlayerEnum player);
        MatchStateDto GetCurrentMatch();
        MatchStateDto StartMatch(PlayerDto? player1 = null, PlayerDto? player2 = null);
        public MatchStateDto SetPlayer1(PlayerDto player);
        public MatchStateDto SetPlayer2(PlayerDto player);
    }
}
