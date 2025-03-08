using LowOnLegs.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowOnLegs.Core.DTOs
{
    public class MatchDto
    {
        public int MatchId { get; set; }
        public PlayerDto? Player1 { get; set; }
        public PlayerDto? Player2 { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsFinished { get; set; }
        public int? WinnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool SaveMatchToDatabase { get; set; }

        public Match ToEntity()
        {
            return new Match
            {
                MatchId = this.MatchId,
                Player1Id = this.Player1?.Id,
                Player2Id = this.Player2?.Id,
                StartTime = this.StartTime,
                EndTime = this.EndTime,
                IsFinished = this.IsFinished,
                WinnerId = this.WinnerId,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt,
                Player1Score = this.Player1Score,
                Player2Score = this.Player2Score
            };
        }
    }
}
