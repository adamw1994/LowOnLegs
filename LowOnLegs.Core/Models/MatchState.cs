using LowOnLegs.Core.DTOs;
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
    }
}
