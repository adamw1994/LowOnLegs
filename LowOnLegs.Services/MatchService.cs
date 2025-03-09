using LowOnLegs.Core.DTOs;
using LowOnLegs.Core.Enums;
using LowOnLegs.Core.Interfaces;
using LowOnLegs.Core.Models;
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
            return matchStateManager.StartMatch(matchStateDto.Player1, matchStateDto.Player2);
        }

        public MatchStateDto ResetMatch()
        {
            var matchStateDto = matchStateManager.GetCurrentMatch();
            matchStateDto.Player1Score = 0;
            matchStateDto.Player2Score = 0;
            return matchStateManager.SetMatchState(matchStateDto);
        }

        public MatchStateDto AddPoint(PlayerEnum player)
        {
            var matchStateDto = matchStateManager.GetCurrentMatch();

            if (matchStateDto.Player1Score == 0
                    && matchStateDto.Player2Score == 0 && matchStateDto.FirstServer == null)
            {
                matchStateDto.FirstServer = player;
                matchStateDto.CurrentServer = player;
                matchStateDto.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                IncreasePlayerScore(player, matchStateDto);
            }

            return matchStateManager.SetMatchState(matchStateDto);
        }

        private static void IncreasePlayerScore(PlayerEnum player, MatchStateDto matchStateDto)
        {
            switch (player)
            {
                case PlayerEnum.Player1:
                    matchStateDto.Player1Score++;
                    break;
                case PlayerEnum.Player2:
                    matchStateDto.Player2Score++;
                    break;
                default:
                    break;
            }

            if ((matchStateDto.Player1Score + matchStateDto.Player2Score) % 2 == 0)
            {
                SwitchCurrentServer(matchStateDto);
            }
        }

        public MatchStateDto SetPlayer1(PlayerDto player)
        {
            var matchStateDto = matchStateManager.GetCurrentMatch();
            matchStateDto.Player1 = player;
            return matchStateManager.SetMatchState(matchStateDto);
        }

        public MatchStateDto SetPlayer2(PlayerDto player)
        {
            var matchStateDto = matchStateManager.GetCurrentMatch();
            matchStateDto.Player2 = player;
            return matchStateManager.SetMatchState(matchStateDto);
        }

        private static void SwitchCurrentServer(MatchStateDto matchStateDto)
        {
            matchStateDto.CurrentServer = matchStateDto.CurrentServer == PlayerEnum.Player1 ? PlayerEnum.Player2 : PlayerEnum.Player1;
        }
    }
}
