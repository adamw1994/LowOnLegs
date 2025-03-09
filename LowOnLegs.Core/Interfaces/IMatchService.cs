using LowOnLegs.Core.DTOs;
using LowOnLegs.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowOnLegs.Core.Interfaces
{
    public interface IMatchService
    {
        MatchStateDto FinishMatch();
        MatchStateDto StartMatch(PlayerDto? player1 = null, PlayerDto? player2 = null);
        MatchStateDto AddPoint(PlayerEnum player);
        public MatchStateDto SetPlayer1(PlayerDto player);
        public MatchStateDto SetPlayer2(PlayerDto player);
         MatchStateDto ResetMatch();
    }
}
