using LowOnLegs.API.Hubs;
using LowOnLegs.Core.DTOs;
using LowOnLegs.Core.Enums;
using LowOnLegs.Core.Interfaces;
using LowOnLegs.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LowOnLegs.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _matchService;
        private readonly IPlayerService _playerService;
        private readonly IHubContext<ScoreboardHub> _hub;

        public MatchesController(IMatchService matchService, IPlayerService playerService, IHubContext<ScoreboardHub> hub)
        {
            _matchService = matchService;
            _playerService = playerService;
            _hub = hub;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartMatch()
        {
            var matchStateDto = _matchService.StartMatch();
            return await Task.FromResult(new JsonResult(matchStateDto));
        }


        [HttpPost("finish")]
        public async Task<IActionResult> FinishMatch()
        {
            var matchStateDto = _matchService.FinishMatch();
            return await Task.FromResult(new JsonResult(matchStateDto));
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetMatch()
        {
            var matchStateDto = _matchService.ResetMatch();
            return await Task.FromResult(new JsonResult(matchStateDto));
        }

        [HttpPost("add-point-player1")]
        public async Task<IActionResult> AddPointPlayer1()
        {
            var matchStateDto = _matchService.AddPoint(PlayerEnum.Player1);
            //await _hub.Clients.All.SendAsync("UpdateScore", new
            //{ player1 = matchStateDto.Player1Score, player2 = matchStateDto.Player2Score }
            //);
            return await Task.FromResult(new JsonResult(matchStateDto));
        }

        [HttpPost("subtract-point-player1")]
        public async Task<IActionResult> SubtractPointPlayer1()
        {
            var matchStateDto = _matchService.SubtractPoint(PlayerEnum.Player1);
            //await _hub.Clients.All.SendAsync("UpdateScore", new
            //{ player1 = matchStateDto.Player1Score, player2 = matchStateDto.Player2Score }
            //);
            return await Task.FromResult(new JsonResult(matchStateDto));
        }

        [HttpPost("add-point-player2")]
        public async Task<IActionResult> AddPointPlayer2()
        {
            var matchStateDto = _matchService.AddPoint(PlayerEnum.Player2);
            //await _hub.Clients.All.SendAsync("UpdateScore", new
            //{ player1 = matchStateDto.Player1Score, player2 = matchStateDto.Player2Score }
            //);
            return await Task.FromResult(new JsonResult(matchStateDto));
        }

        [HttpPost("subtract-point-player2")]
        public async Task<IActionResult> SubtractPointPlayer2()
        {
            var matchStateDto = _matchService.SubtractPoint(PlayerEnum.Player2);
            //await _hub.Clients.All.SendAsync("UpdateScore", new
            //{ player1 = matchStateDto.Player1Score, player2 = matchStateDto.Player2Score }
            //);
            return await Task.FromResult(new JsonResult(matchStateDto));
        }

        [HttpPost("set-player1/{playerId}")]
        public async Task<IActionResult> SetPlayer1(int playerId)
        {
            var playerDto = _playerService.GetPlayer(playerId);
            var matchStateDto = _matchService.SetPlayer1(playerDto);
            return await Task.FromResult(new JsonResult(matchStateDto));
        }

        [HttpPost("set-player2/{playerId}")]
        public async Task<IActionResult> SetPlayer2(int playerId)
        {
            var playerDto = _playerService.GetPlayer(playerId);
            var matchStateDto = _matchService.SetPlayer2(playerDto);
            return await Task.FromResult(new JsonResult(matchStateDto));
        }
    }
}
