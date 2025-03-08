using LowOnLegs.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowOnLegs.Core.DTOs
{
    public class MatchStateDto
    {
        public int MatchId { get; set; }
        public PlayerDto? Player1 { get; set; }
        public PlayerDto? Player2 { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public MatchStateDto(MatchState matchState)
        {
            MatchId = matchState.MatchId;
            Player1 = matchState.Player1;
            Player2 = matchState.Player2;
            Player1Score = matchState.Player1Score;
            Player2Score = matchState.Player2Score;
            StartTime = matchState.StartTime;
            CreatedAt = matchState.CreatedAt;
            UpdatedAt = matchState.UpdatedAt;
        }

        public MatchDto ToMatchDto(MatchStateDto matchStateDto)
        {
            var matchDto =  new MatchDto
            {
                MatchId = matchStateDto.MatchId,
                Player1 = matchStateDto.Player1,
                Player2 = matchStateDto.Player2,
                Player1Score = matchStateDto.Player1Score,
                Player2Score = matchStateDto.Player2Score,
                StartTime = matchStateDto.StartTime,
                CreatedAt = matchStateDto.CreatedAt,
                UpdatedAt = matchStateDto.UpdatedAt,
                EndTime = DateTime.Now,
                IsFinished = true,
                SaveMatchToDatabase = matchStateDto.Player1 is not null && matchStateDto.Player2 is not null
            };

            if (!matchDto.SaveMatchToDatabase)
                return matchDto;

            if (matchStateDto.Player1Score > matchStateDto.Player2Score)
            {
                matchDto.WinnerId = matchStateDto.Player1?.Id;
            }
            else if (matchStateDto.Player1Score < matchStateDto.Player2Score)
            {
                matchDto.WinnerId = matchStateDto.Player2?.Id;
            }
            return matchDto;
        }
    }
}
