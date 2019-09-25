using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GuyVdN.Neat;
using GuyVdN.Neat.Racing;

namespace Neat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const bool playYourself = false;
        private readonly Game manualGame = new Game();
        private int generation = 0;
        private int numberOfGenerationsToTrain = 1;
        private Dictionary<int, Game> games;
        private readonly Population population;
        private readonly GameControl[] gameControls = new GameControl[5];
        private readonly GenomeControl[] genomeControls = new GenomeControl[5];
        private readonly BackgroundWorker backGroundWorker = new BackgroundWorker { WorkerReportsProgress = true };

        public MainWindow()
        {
            InitializeComponent();

            const int numberOfPlayers = 150;
            const int numberOfInputNodes = 6;
            const int numberOfOutputNodes = 3;

            population = new Population(numberOfPlayers, numberOfInputNodes, numberOfOutputNodes, NewPlayerAction.AddRandomConnection)
            {
                GetPlayerInput = GetPlayerInput,
                GetPlayerScore = GetPlayerScore,
                GetPlayerFitness = GetPlayerFitness,
                ProcessOutput = ProcessOutput
            };

            gameControls[0] = GameControl0;
            gameControls[1] = GameControl1;
            gameControls[2] = GameControl2;
            gameControls[3] = GameControl3;
            gameControls[4] = GameControl4;

            genomeControls[0] = GenomeControl0;
            genomeControls[1] = GenomeControl1;
            genomeControls[2] = GenomeControl2;
            genomeControls[3] = GenomeControl3;
            genomeControls[4] = GenomeControl4;

            backGroundWorker.DoWork += Train;
            backGroundWorker.ProgressChanged += ShowProgress;
            backGroundWorker.RunWorkerCompleted += CompletedTraining;
        }

        private double[] GetPlayerInput(int playerNumber)
        {
            if (!games.ContainsKey(playerNumber))
                games[playerNumber] = new Game();

            return games[playerNumber].Data.Cast<int>().Select(Convert.ToDouble).Skip(6).ToArray();
        }

        private double GetPlayerScore(Player player)
        {
            return games[player.Number].Score;
        }

        private double GetPlayerFitness(Player player)
        {
            var score = Math.Max(0, games[player.Number].Score);
            return (float)score * score / player.Age;
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
            }
            else if (maxIndex == 1)
            {
                game.MoveRight();
            }
            else if (maxIndex == 2)
            {
                game.MoveForward();
            }

            if (game.GameOver)
                return -1;

            return game.Finished ? 1 : 0;
        }

        private void ShowProgress(object sender, ProgressChangedEventArgs e)
        {
            GenerationValue.Content = generation;
            TrainingProgress.Value = e.ProgressPercentage;
            SpeciesValue.Content = population.Species.Count();
        }

        private void Train()
        {
            if (generation > 0)
            {
                population.Evolve();
            }

            backGroundWorker.RunWorkerAsync();
        }

        private void Train(object sender, DoWorkEventArgs e)
        {
            for (var i = 1; i <= numberOfGenerationsToTrain; i++)
            {
                generation++;
                backGroundWorker.ReportProgress(i * 100 / numberOfGenerationsToTrain);

                games = new Dictionary<int, Game>();

                while (population.IsAlive)
                {
                    population.MovePlayers();

                    if (population.Players.Any(p => p.IsWinner))
                        return;
                }

                if (i < numberOfGenerationsToTrain)
                {
                    population.Evolve();
                }
            }

            if (numberOfGenerationsToTrain < 1000)
                numberOfGenerationsToTrain *= 2;
        }

        private void CompletedTraining(object sender, RunWorkerCompletedEventArgs e)
        {
            ShowBestPlayerGames();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (playYourself)
            {
                switch (e.Key)
                {
                    case Key.Left:
                        manualGame.MoveLeft();
                        break;
                    case Key.Right:
                        manualGame.MoveRight();
                        break;
                    case Key.Up:
                        manualGame.MoveForward();
                        break;
                    case Key.S:
                        manualGame.Start();
                        break;
                }

                GameControl0.UpdateGame(manualGame);
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        Train();
                        break;
                    case Key.R: // replay
                        ShowBestPlayerGames();
                        break;
                }
            }

            e.Handled = true;
        }

        private void ShowBestPlayerGames()
        {
            const int numberOfGamesToShow = 5;
            var bestPlayers = population.Players.OrderByDescending(p => p.Fitness).Take(numberOfGamesToShow).ToArray();
            var bestGames = new Game[numberOfGamesToShow];
            var allGamesEnded = false;
            var step = 0;

            numberOfGamesToShow.Times(i =>
            {
                var player = bestPlayers[i];
                bestGames[i] = games.Single(g => g.Key == player.Number).Value;
                gameControls[i].UpdatePlayer(player);
                genomeControls[i].DrawGenome(player.Genome);
            });

            while (!allGamesEnded)
            {
                allGamesEnded = true;

                Thread.Sleep(200);

                for (var i = 0; i < numberOfGamesToShow; i++)
                {
                    var game = bestGames[i];
                    if (game.GameHistory.Length <= step)
                        continue;

                    var gameHistory = game.GameHistory[step];

                    gameControls[i].UpdateGame(gameHistory);

                    if (!gameHistory.GameOver && gameHistory.Finish != 0)
                        allGamesEnded = false;
                }

                Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);

                step++;
            }
        }
    }
}
