using System;
using System.Collections.Generic;
using System.Linq;
using GuyVdN.Neat.Racing;
using NUnit.Framework;
using Shouldly;

namespace GuyVdN.Neat.Test
{
    public class ItShouldBeAbleToPlayAGame
    {
        private Dictionary<int, Game> games;
        private readonly List<int> playersThatMovedLeft = new List<int>();
        private readonly List<int> playersThatMovedRight = new List<int>();
        private readonly List<int> playersThatMovedForward = new List<int>();

        [Test]
        public void With_a_score_greater_than_100()
        {
            const int numberOfPlayers = 150;
            const int numberOfInputNodes = 12;
            const int numberOfOutputNodes = 3;

            Player winner = null;

            var population = new Population(numberOfPlayers, numberOfInputNodes, numberOfOutputNodes, NewPlayerAction.FullyConnect)
            {
                GetPlayerInput = GetPlayerInput,
                GetPlayerScore = GetPlayerScore,
                GetPlayerFitness = GetPlayerFitness,
                ProcessOutput = ProcessOutput
            };

            var generation = 0;

            while (generation < 1000)
            {
                // Create a game for each player
                games = new Dictionary<int, Game>();

                // Let players move until all dead or fail-safe is reached
                var numberOfMovesFailSave = 0;
                while (population.IsAlive && numberOfMovesFailSave < 500)
                {
                    population.MovePlayers();
                    numberOfMovesFailSave++;

                    if (population.Players.Any(p => p.IsWinner))
                    {
                        winner = population.Players.First(p => p.IsWinner);
                        Console.WriteLine("We have a winner!");
                        break;
                    }
                }

                if (winner != null)
                {
                    break;
                }

                if (generation < 999)
                {
                    population.Evolve();
                }

                generation++;
            }

            var bestPlayer = population.BestPlayer;
            Console.WriteLine($"Best player has a score of {bestPlayer.Score} at generation {generation}");
            Console.WriteLine(bestPlayer);

            bestPlayer.Score.ShouldBeGreaterThanOrEqualTo(100);
        }

        private double GetPlayerFitness(Player player)
        {
            var playerNumber = player.Number;
            var multiplier = 0;
            if (playersThatMovedLeft.Contains(playerNumber)) multiplier++;
            if (playersThatMovedRight.Contains(playerNumber)) multiplier++;
            if (playersThatMovedForward.Contains(playerNumber)) multiplier++;
            
            //return Math.Max(0, games[playerNumber].Score * multiplier / (player.Genome.Layers + player.Genome.Nodes.Count() / 10d));
            return Math.Max(0, 20 + games[player.Number].Score * player.Age * multiplier);
        }

        private double[] GetPlayerInput(int playerNumber)
        {
            if (!games.ContainsKey(playerNumber))
                games[playerNumber] = new Game();


            return games[playerNumber].Data.Cast<int>().Select(Convert.ToDouble).ToArray();
        }

        private double GetPlayerScore(Player player)
        {
            return games[player.Number].Score;
        }

        private int ProcessOutput(Player player, double[] output)
        {
            var playerNumber = player.Number;
            var maxValue = output.Max();
            var maxIndex = output.ToList().IndexOf(maxValue);

            var game = games[playerNumber];

            if (maxIndex == 0)
            {
                game.MoveLeft();
                if (!playersThatMovedLeft.Contains(playerNumber))
                    playersThatMovedLeft.Add(playerNumber);
            }
            else if (maxIndex == 1)
            {
                game.MoveRight();
                if (!playersThatMovedRight.Contains(playerNumber))
                    playersThatMovedRight.Add(playerNumber);
            }
            else if (maxIndex == 2)
            {
                game.MoveForward();
                if (!playersThatMovedForward.Contains(playerNumber))
                    playersThatMovedForward.Add(playerNumber);
            }

            if (game.GameOver)
                return -1;

            if (game.Finished)
                return 1;

            return 0;
        }
    }
}