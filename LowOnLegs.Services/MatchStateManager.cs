using LowOnLegs.Core.DTOs;
using LowOnLegs.Core.Enums;
using LowOnLegs.Core.Interfaces;
using LowOnLegs.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowOnLegs.Services
{
    public class MatchStateManager : IMatchStateManager
    {
        private MatchState? _currentMatch;
        private readonly object _lockObject = new object();

        public MatchStateDto StartMatch(PlayerDto? player1, PlayerDto? player2)
        {
            lock (_lockObject)
            {
                _currentMatch = new MatchState
                {
                    StartTime = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Player1Score = 0,
                    Player2Score = 0,
                    Player1 = player1,
                    Player2 = player2,
                };

                return new MatchStateDto(_currentMatch);
            }
        }

        public MatchStateDto AddPoint(PlayerEnum player)
        {
            lock (_lockObject)
            {
                if (_currentMatch == null)
                {
                    throw new Exception("No match is currently in progress");
                }

                if (player == PlayerEnum.Player1)
                {
                    _currentMatch.Player1Score++;
                }
                else if (player == PlayerEnum.Player2)
                {
                    _currentMatch.Player2Score++;
                }

                _currentMatch.UpdatedAt = DateTime.UtcNow;

                return new MatchStateDto(_currentMatch);
            }
        }

        public MatchStateDto GetCurrentMatch()
        {
            lock (_lockObject)
            {
                if (_currentMatch is null)
                {
                    throw new Exception("No match is currently in progress");
                }

                return new MatchStateDto(_currentMatch);
            }
        }

        public MatchStateDto SetPlayer1(PlayerDto player)
        {
            lock (_lockObject)
            {
                if (_currentMatch is null)
                {
                    throw new Exception("No match is currently in progress");
                }
                
                _currentMatch.Player1 = player;
                return new MatchStateDto(_currentMatch);
            }
        }

        public MatchStateDto SetPlayer2(PlayerDto player)
        {
            lock (_lockObject)
            {
                if (_currentMatch is null)
                {
                    throw new Exception("No match is currently in progress");
                }

                _currentMatch.Player2 = player;
                return new MatchStateDto(_currentMatch);
            }
        }
    }

}
