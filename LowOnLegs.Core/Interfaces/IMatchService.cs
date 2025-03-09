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
        MatchStateDto SubtractPoint(PlayerEnum player);
        MatchStateDto SetPlayer1(PlayerDto player);
        MatchStateDto SetPlayer2(PlayerDto player);
         MatchStateDto ResetMatch();
    }
}
