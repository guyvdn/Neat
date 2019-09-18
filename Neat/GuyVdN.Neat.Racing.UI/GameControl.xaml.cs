using System.Windows.Controls;
using System.Windows;
using GuyVdN.Neat;
using GuyVdN.Neat.Racing;

namespace Neat
{
    /// <summary>
    /// Interaction logic for GameControl.xaml
    /// </summary>
    public partial class GameControl : UserControl
    {
        public GameControl()
        {
            InitializeComponent();
        }

        public void UpdatePlayer(Player player)
        {
            AgeValue.Content = player.Age;
            FitnessValue.Content = player.Fitness;
            GameInfo.Text = player.Genome.ToString();
        }

        public void UpdateGame(GameHistory gameHistory)
        {
            var gameData = gameHistory.Data;

            RedCar00.Visibility = RedCarVisibility(gameData[0, 0]);
            RedCar01.Visibility = RedCarVisibility(gameData[0, 1]);
            RedCar02.Visibility = RedCarVisibility(gameData[0, 2]);

            RedCar10.Visibility = RedCarVisibility(gameData[1, 0]);
            RedCar11.Visibility = RedCarVisibility(gameData[1, 1]);
            RedCar12.Visibility = RedCarVisibility(gameData[1, 2]);

            RedCar20.Visibility = RedCarVisibility(gameData[2, 0]);
            RedCar21.Visibility = RedCarVisibility(gameData[2, 1]);
            RedCar22.Visibility = RedCarVisibility(gameData[2, 2]);

            RedCar30.Visibility = RedCarVisibility(gameData[3, 0]);
            RedCar31.Visibility = RedCarVisibility(gameData[3, 1]);
            RedCar32.Visibility = RedCarVisibility(gameData[3, 2]);

            YellowCar0.Visibility = YellowCarVisibility(gameData[3, 0]);
            YellowCar1.Visibility = YellowCarVisibility(gameData[3, 1]);
            YellowCar2.Visibility = YellowCarVisibility(gameData[3, 2]);

            Bang0.Visibility = BangVisibility(gameData[3, 0]);
            Bang1.Visibility = BangVisibility(gameData[3, 1]);
            Bang2.Visibility = BangVisibility(gameData[3, 2]);

            ScoreValue.Content = gameHistory.Score;
            FinishValue.Content = gameHistory.Finish;
            GameOver.Visibility = gameHistory.GameOver.ToVisibility();
            Finished.Visibility = gameHistory.Finished.ToVisibility();
        }

        public void UpdateGame(Game game)
        {
            RedCar00.Visibility = RedCarVisibility(game.Data[0, 0]);
            RedCar01.Visibility = RedCarVisibility(game.Data[0, 1]);
            RedCar02.Visibility = RedCarVisibility(game.Data[0, 2]);
                                                       
            RedCar10.Visibility = RedCarVisibility(game.Data[1, 0]);
            RedCar11.Visibility = RedCarVisibility(game.Data[1, 1]);
            RedCar12.Visibility = RedCarVisibility(game.Data[1, 2]);
                                                       
            RedCar20.Visibility = RedCarVisibility(game.Data[2, 0]);
            RedCar21.Visibility = RedCarVisibility(game.Data[2, 1]);
            RedCar22.Visibility = RedCarVisibility(game.Data[2, 2]);
                                                       
            RedCar30.Visibility = RedCarVisibility(game.Data[3, 0]);
            RedCar31.Visibility = RedCarVisibility(game.Data[3, 1]);
            RedCar32.Visibility = RedCarVisibility(game.Data[3, 2]);

            YellowCar0.Visibility = YellowCarVisibility(game.Data[3, 0]);
            YellowCar1.Visibility = YellowCarVisibility(game.Data[3, 1]);
            YellowCar2.Visibility = YellowCarVisibility(game.Data[3, 2]);

            Bang0.Visibility = BangVisibility(game.Data[3, 0]);
            Bang1.Visibility = BangVisibility(game.Data[3, 1]);
            Bang2.Visibility = BangVisibility(game.Data[3, 2]);

            ScoreValue.Content = game.Score;
            FinishValue.Content = game.Finish;
            GameOver.Visibility = game.GameOver.ToVisibility();
            Finished.Visibility = game.Finished.ToVisibility();
        }

        private static Visibility RedCarVisibility(int value) => (value == -1).ToVisibility();
        private static Visibility YellowCarVisibility(int value) => (value >= 1).ToVisibility();
        private static Visibility BangVisibility(int value) => (value == 2).ToVisibility();
    }
}