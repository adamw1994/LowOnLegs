using LowOnLegs.Core.DTOs;
using LowOnLegs.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowOnLegs.Core.Models
{
    public class MatchState
    {
        public int MatchId { get; set; }
        public PlayerDto? Player1 { get; set; }
        public PlayerDto? Player2 { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PlayerEnum? FirstServer { get; set; }
        public PlayerEnum? CurrentServer { get; set; }

        public MatchState(PlayerDto? player1 = null, PlayerDto? player2 = null)
        {
            StartTime = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Player1Score = 0;
            Player2Score = 0;
            Player1 = player1;
            Player2 = player2;
        }

        public MatchState(MatchStateDto dto)
        {
            StartTime = dto.StartTime;
            CreatedAt = dto.CreatedAt;
            UpdatedAt = dto.UpdatedAt;
            Player1Score = dto.Player1Score;
            Player2Score = dto.Player2Score;
            Player1 = dto.Player1;
            Player2 = dto.Player2;
            FirstServer = dto.FirstServer;
            CurrentServer = dto.CurrentServer;

        }
    }
}
