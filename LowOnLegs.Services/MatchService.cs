using LowOnLegs.Core.DTOs;
using LowOnLegs.Core.Enums;
using LowOnLegs.Core.Interfaces;
using LowOnLegs.Data;
using LowOnLegs.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowOnLegs.Services
{
    public class MatchService : IMatchService
    {
        private IMatchRepository matchRepository;
        private IMatchStateManager matchStateManager;

        public MatchService(IMatchRepository matchRepository, IMatchStateManager matchStateManager)
        {
            this.matchRepository = matchRepository;
            this.matchStateManager = matchStateManager;
        }

        public MatchStateDto StartMatch(PlayerDto? player1 = null, PlayerDto? player2 = null)
        {
            return matchStateManager.StartMatch(player1, player2);
        }

        public MatchStateDto FinishMatch()
        {
            var matchStateDto = matchStateManager.GetCurrentMatch();
            var matchDto = matchStateDto.ToMatchDto(matchStateDto);

            if (matchDto.SaveMatchToDatabase)
            {
                matchRepository.Add(matchDto.ToEntity());
            }

            return matchStateManager.StartMatch();
        }

        public MatchStateDto AddPoint(PlayerEnum player)
        {
            return matchStateManager.AddPoint(player);
        }

        public MatchStateDto SetPlayer1(PlayerDto player)
        {
            return matchStateManager.SetPlayer1(player);
        }

        public MatchStateDto SetPlayer2(PlayerDto player)
        {
            return matchStateManager.SetPlayer2(player);
        }
    }
}
