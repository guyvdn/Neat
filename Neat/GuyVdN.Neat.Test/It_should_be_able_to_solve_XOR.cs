using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace GuyVdN.Neat.Test
{
    /// <summary>
    /// A | B | Out
    /// ---------------
    /// 0 | 0 | 0
    /// 0 | 1 | 1
    /// 1 | 0 | 1
    /// 1 | 1 | 0
    /// </summary>


    [TestFixture]
    public class It_should_be_able_to_solve_XOR
    {
        // To keep track of the questions each player has received
        private Dictionary<int, int> playerQuestions;
        private Dictionary<Player, int> playerScores;
        private Dictionary<int, double> playerFitness;

        [Test]
        public void Within_100_generations()
        {
            const int numberOfPlayers = 1000;
            const int numberOfInputNodes = 2;
            const int numberOfOutputNodes = 1;

            Player winner = null;

            var population = new Population(numberOfPlayers, numberOfInputNodes, numberOfOutputNodes, NewPlayerAction.AddRandomConnection)
            {
                GetPlayerInput = GetPlayerInput,
                GetPlayerScore = GetPlayerScore,
                GetPlayerFitness = GetPlayerFitness,
                ProcessOutput = ProcessOutput
            };

            var generation = 0;
            while (generation < 100)
            {
                // Clear the questions and scores
                playerQuestions = new Dictionary<int, int>();
                playerScores = new Dictionary<Player, int>();
                playerFitness = new Dictionary<int, double>();

                // Let all players make the 4 possible moves
                for (var i = 0; i < 4; i++)
                {
                    population.MovePlayers();
                }

                if (playerScores.Any(s => s.Value == 4)) // we might have a winner!
                {
                    winner = playerScores.Where(s => s.Value == 4).Select(x => x.Key).FirstOrDefault();

                    if (winner != null)
                    {
                        Console.WriteLine("Winner winner chicken dinner!");
                        break;
                    }
                }

                Console.WriteLine($"Best score after generation {generation} is {population.BestScore}");
                Console.WriteLine($"Best fitness after generation {generation} is {population.BestFitness}");
                Console.WriteLine($"Number of species {population.Species.Count()}");
                Console.WriteLine("---");

                population.Evolve();
                generation++;
            }

            winner.ShouldNotBeNull();

            Console.WriteLine(winner);

            winner.ShouldSatisfyAllConditions(
                () => winner.Move(new[] { 0d, 0d })[0].ShouldBeLessThan(0.5),
                () => winner.Move(new[] { 0d, 1d })[0].ShouldBeGreaterThan(0),
                () => winner.Move(new[] { 1d, 0d })[0].ShouldBeGreaterThan(0),
                () => winner.Move(new[] { 1d, 1d })[0].ShouldBeLessThan(0.5)
            );
        }

        private double GetPlayerFitness(Player player)
        {
            return playerFitness[player.Number];
        }

        private double[] GetPlayerInput(int playerNumber)
        {
            var nextQuestion = 1;

            if (playerQuestions.ContainsKey(playerNumber))
            {
                nextQuestion = playerQuestions[playerNumber] + 1;
            }

            playerQuestions[playerNumber] = nextQuestion;

            switch (nextQuestion)
            {
                case 1: return new[] { 0d, 0d };
                case 2: return new[] { 0d, 1d };
                case 3: return new[] { 1d, 0d };
                case 4: return new[] { 1d, 1d };
            }

            throw new Exception("Not supposed to get here");
        }

        private int ProcessOutput(Player player, double[] output)
        {
            var playerNumber = player.Number;
            var question = playerQuestions[playerNumber];
            var answer = output[0];
            var score = false;
            var fitness = 0d;

            switch (question)
            {
                case 1:
                    {
                        score = answer < 0.5d;
                        fitness = 1 - answer;
                        break;
                    }
                case 2:
                    {
                        score = answer > 0.5d;
                        fitness = answer;
                        break;
                    }
                case 3:
                    {
                        score = answer > 0.5d;
                        fitness = answer;
                        break;
                    }
                case 4:
                    {
                        score = answer < 0.5d;
                        fitness = 1 - answer;
                        break;
                    }
            }

            if (!playerScores.ContainsKey(player))
                playerScores[player] = 0;

            if (!playerFitness.ContainsKey(playerNumber))
                playerFitness[playerNumber] = 0;

            if (score)
                playerScores[player] += 1;

            playerFitness[playerNumber] += fitness * fitness / (player.Genome.Layers + player.Genome.Nodes.Count() + player.Genome.Connections.Count());
            //playerFitness[playerNumber] += fitness * fitness;/// (player.Genome.Layers + player.Genome.Nodes.Count() + player.Genome.Connections.Count());

            return 0;
        }


        private double GetPlayerScore(Player player)
        {
            return playerScores[player];
        }
    }
}