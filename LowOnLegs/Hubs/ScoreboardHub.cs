using Microsoft.AspNetCore.SignalR;

namespace LowOnLegs.API.Hubs
{
    public class ScoreboardHub: Hub
    {
        public async Task SendScoreUpdate(int player1Score, int player2Score)
        {
            await Clients.All.SendAsync("UpdateScore", new { player1 = player1Score, player2 = player2Score });
        }
    }
}
