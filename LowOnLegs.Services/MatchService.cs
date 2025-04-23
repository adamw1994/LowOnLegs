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

        public MatchStateDto StartMatch(PlayerDto? leftPlayer = null, PlayerDto? rightPlayer = null)
        {
            return matchStateManager.StartMatch(leftPlayer, rightPlayer);
        }

        public MatchStateDto FinishMatch()
        {
            var matchStateDto = matchStateManager.GetCurrentMatch();
            var matchDto = matchStateDto.ToMatchDto(matchStateDto);

            if (matchDto.SaveMatchToDatabase)
            {
                matchRepository.Add(matchDto.ToEntity());
            }
            return matchStateManager.StartMatch(matchStateDto.LeftPlayer, matchStateDto.RightPlayer);
        }

        public MatchStateDto ResetMatch()
        {
            var matchStateDto = matchStateManager.GetCurrentMatch();
            matchStateDto.LeftPlayerScore = 0;
            matchStateDto.RightPlayerScore = 0;
            matchStateDto.FirstServer = null;
            matchStateDto.CurrentServer = null;
            return matchStateManager.SetMatchState(matchStateDto);
        }

        public MatchStateDto AddPoint(PlayerEnum player)
        {
            var matchStateDto = matchStateManager.GetCurrentMatch();

            if (IsFightForServe(matchStateDto))
            {
                InitializeFirstServer(matchStateDto, player);
            }
            else
            {
                IncreasePlayerScore(player, matchStateDto);
            }

            return matchStateManager.SetMatchState(matchStateDto);
        }

        public MatchStateDto SubtractPoint(PlayerEnum player)
        {
            var matchStateDto = matchStateManager.GetCurrentMatch();

            DecreasePlayerScore(player, matchStateDto);

            return matchStateManager.SetMatchState(matchStateDto);
        }

        private void IncreasePlayerScore(PlayerEnum player, MatchStateDto matchStateDto)
        {
            switch (player)
            {
                case PlayerEnum.Left:
                    matchStateDto.LeftPlayerScore++;
                    if (IsTimeToSwitchServer(matchStateDto, PointOperation.Add))
                    {
                        SwitchCurrentServer(matchStateDto);
                    }
                    break;
                case PlayerEnum.Right:
                    matchStateDto.RightPlayerScore++;
                    if (IsTimeToSwitchServer(matchStateDto, PointOperation.Add))
                    {
                        SwitchCurrentServer(matchStateDto);
                    }
                    break;
                default:
                    break;
            }
        }

        private void DecreasePlayerScore(PlayerEnum player, MatchStateDto matchStateDto)
        {
            switch (player)
            {
                case PlayerEnum.Left:
                    if (matchStateDto.LeftPlayerScore > 0)
                    {
                        matchStateDto.LeftPlayerScore--;
                        if (IsTimeToSwitchServer(matchStateDto, PointOperation.Subtract))
                        {
                            SwitchCurrentServer(matchStateDto);
                        }
                    }

                    break;
                case PlayerEnum.Right:
                    if (matchStateDto.RightPlayerScore > 0)
                    {
                        matchStateDto.RightPlayerScore--;
                        if (IsTimeToSwitchServer(matchStateDto, PointOperation.Subtract))
                        {
                            SwitchCurrentServer(matchStateDto);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private bool IsTimeToSwitchServer(MatchStateDto matchStateDto, PointOperation operation)
        {
            var scoreComparer = operation == PointOperation.Add ? 0 : 1;
            return (matchStateDto.LeftPlayerScore + matchStateDto.RightPlayerScore) % 2 == scoreComparer;
        }

        private bool IsFightForServe(MatchStateDto matchStateDto)
        {
            return matchStateDto.LeftPlayerScore == default &&
                   matchStateDto.RightPlayerScore == default &&
                   matchStateDto.FirstServer is null;
        }

        private void InitializeFirstServer(MatchStateDto matchStateDto, PlayerEnum player)
        {
            matchStateDto.FirstServer = player;
            matchStateDto.CurrentServer = player;
            matchStateDto.UpdatedAt = DateTime.UtcNow;
        }

        public MatchStateDto SetLeftPlayer(PlayerDto player)
        {
            var matchStateDto = matchStateManager.GetCurrentMatch();
            matchStateDto.LeftPlayer = player;
            return matchStateManager.SetMatchState(matchStateDto);
        }

        public MatchStateDto SetRightPlayer(PlayerDto player)
        {
            var matchStateDto = matchStateManager.GetCurrentMatch();
            matchStateDto.RightPlayer = player;
            return matchStateManager.SetMatchState(matchStateDto);
        }

        private static void SwitchCurrentServer(MatchStateDto matchStateDto)
        {
            matchStateDto.CurrentServer = matchStateDto.CurrentServer == PlayerEnum.Left 
                ? PlayerEnum.Right
                : PlayerEnum.Left;
        }
    }
}
