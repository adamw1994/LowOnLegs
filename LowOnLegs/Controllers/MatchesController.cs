using LowOnLegs.Core.DTOs;
using LowOnLegs.Core.Enums;
using LowOnLegs.Core.Interfaces;
using LowOnLegs.Services;
using Microsoft.AspNetCore.Mvc;

namespace LowOnLegs.API.Controllers
{
    public class MatchesController
    {
        private readonly IMatchService _matchService;
        private readonly IPlayerService _playerService;

        public MatchesController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        public async Task<IActionResult> StartMatch()
        {
            var matchStateDto = _matchService.StartMatch();
            return new JsonResult(true);
        }

        public async Task<IActionResult> FinishMatch()
        {
            _matchService.FinishMatch();
            return new JsonResult(true);
        }

        public async Task<IActionResult> AddPointPlayer1()
        {
            _matchService.AddPoint(PlayerEnum.Player1);
            return new JsonResult(true);
        }

        public async Task<IActionResult> AddPointPlayer2()
        {
            _matchService.AddPoint(PlayerEnum.Player2);
            return new JsonResult(true);
        }

        public async Task<IActionResult> SetPlayer1(int playerId)
        {
            var playerDto = _playerService.GetPlayer(playerId);
            _matchService.SetPlayer1(playerDto);
            return new JsonResult(true);
        }

        public async Task<IActionResult> SetPlayer2(int playerId)
        {
            var playerDto = _playerService.GetPlayer(playerId);
            _matchService.SetPlayer2(playerDto);
            return new JsonResult(true);
        }
    }
}
