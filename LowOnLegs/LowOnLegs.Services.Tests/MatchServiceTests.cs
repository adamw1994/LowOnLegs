using LowOnLegs.Core.DTOs;
using LowOnLegs.Data.Repositories.Interfaces;
using NSubstitute;

namespace LowOnLegs.Services.Tests;

public class MatchServiceTests
{
    [Fact]
    public async Task FinishMatch_WhenScoreIs11To10_ShouldRejectWithoutSideEffects()
    {
        //Arrange
            var leftPlayerDto = new PlayerDto { Id = 1, Name = "Player 1" };
            var rightPlayerDto = new PlayerDto { Id = 2, Name = "Player 2" };
            var matchStateManager = new MatchStateManager();
            var currentState = matchStateManager.StartMatch(leftPlayerDto, rightPlayerDto);
            currentState.LeftPlayerScore = 11;
            currentState.RightPlayerScore = 10;
            
            matchStateManager.SetMatchState(currentState);

            var matchRepository = Substitute.For<IMatchRepository>();
            var playerRepository = Substitute.For<IPlayerRepository>();

            var service = new MatchService(matchRepository, matchStateManager, playerRepository);
        //Act

        //Assert


    }
}
