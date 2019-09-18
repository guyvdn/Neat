using System;
using System.Collections.Generic;

namespace GuyVdN.Neat.Racing
{
    public class GameHistory
    {
        public int[,] Data { get; set; }
        public int Score { get; set; }
        public int Finish { get; set; }
        public bool GameOver { get; set; }
        public bool Finished => Finish <= 0;
    }

    public class Game
    {
        private readonly List<GameHistory> gameHistory = new List<GameHistory>();
        public GameHistory[] GameHistory => gameHistory.ToArray();

        private static readonly Random random = new Random();

        private int numberOfMovesSinceLastForward;

        public int[,] Data { get; private set; }

        public int Finish { get; private set; }

        public bool Finished => Finish == 0;

        public int Score { get; private set; }

        public bool GameOver { get; private set; }

        public Game()
        {
            Start();
        }

        public void Start()
        {
            Data = new int[4, 3];
            GameOver = false;
            Score = 0;
            Finish = 200;

            var dataRow = GenerateDataRow();

            Data[0, 0] = dataRow[0];
            Data[0, 1] = dataRow[1];
            Data[0, 2] = dataRow[2];

            dataRow = GenerateDataRow();

            Data[2, 0] = dataRow[0];
            Data[2, 1] = dataRow[1];
            Data[2, 2] = dataRow[2];

            Data[3, 1] = 1;
        }

        private static int[] GenerateDataRow()
        {
            var rowType = random.Next(6);

            switch (rowType)
            {
                case 0: return new[] { -1, 0, 0 };
                case 1: return new[] { 0, -1, 0 };
                case 2: return new[] { 0, 0, -1 };
                case 3: return new[] { -1, -1, 0 };
                case 4: return new[] { 0, -1, -1 };
                case 5: return new[] { -1, 0, -1 };
            }

            throw new IndexOutOfRangeException();
        }

        public void MoveLeft()
        {
            if (GameOver || Finished)
                return;

            var couldHaveMovedForward = false;

            numberOfMovesSinceLastForward++;

            if (Data[3, 0] == 1)
            {
                Data[3, 0] = 2;
                GameOver = true;
                if (Data[2, 0] == 0)
                    couldHaveMovedForward = true;
            }

            if (Data[3, 1] == 1)
            {
                if (Data[3, 0] == -1 || numberOfMovesSinceLastForward > 5)
                {
                    Data[3, 0] = 2;
                    GameOver = true;
                }
                else
                {
                    Data[3, 0] = 1;
                }

                Data[3, 1] = 0;

                if (Data[2, 1] == 0)
                    couldHaveMovedForward = true;
            }

            if (Data[3, 2] == 1)
            {
                if (Data[3, 1] == -1 || numberOfMovesSinceLastForward > 5)
                {
                    Data[3, 1] = 2;
                    GameOver = true;
                }
                else
                {
                    Data[3, 1] = 1;
                }

                Data[3, 2] = 0;

                if (Data[2, 2] == 0)
                    couldHaveMovedForward = true;
            }

            Score = GameOver || couldHaveMovedForward ? Score - 1 : Score;

            AddHistory();
        }

        public void MoveRight()
        {
            if (GameOver || Finished)
                return;

            var couldHaveMovedForward = false;

            numberOfMovesSinceLastForward++;

            if (Data[3, 2] == 1)
            {
                Data[3, 2] = 2;
                GameOver = true;

                if (Data[2, 2] == 0)
                    couldHaveMovedForward = true;
            }

            if (Data[3, 1] == 1)
            {
                if (Data[3, 2] == -1 || numberOfMovesSinceLastForward > 5)
                {
                    Data[3, 2] = 2;
                    GameOver = true;
                }
                else
                {
                    Data[3, 2] = 1;
                }

                Data[3, 1] = 0;

                if (Data[2, 1] == 0)
                    couldHaveMovedForward = true;
            }

            if (Data[3, 0] == 1)
            {
                if (Data[3, 1] == -1 || numberOfMovesSinceLastForward > 5)
                {
                    Data[3, 1] = 2;
                    GameOver = true;
                }
                else
                {
                    Data[3, 1] = 1;
                }

                Data[3, 0] = 0;

                if (Data[2, 0] == 0)
                    couldHaveMovedForward = true;
            }

            Score = GameOver || couldHaveMovedForward ? Score - 1 : Score;

            AddHistory();
        }

        public void MoveForward()
        {
            if (GameOver || Finished)
                return;

            numberOfMovesSinceLastForward = 0;

            if (Data[3, 0] == 1 && Data[2, 0] == -1)
            {
                Data[3, 0] = 2;
                GameOver = true;
            }
            else if (Data[3, 0] != 1)
            {
                Data[3, 0] = Data[2, 0];
            }

            if (Data[3, 1] == 1 && Data[2, 1] == -1)
            {
                Data[3, 1] = 2;
                GameOver = true;
            }
            else if (Data[3, 1] != 1)
            {
                Data[3, 1] = Data[2, 1];
            }

            if (Data[3, 2] == 1 && Data[2, 2] == -1)
            {
                Data[3, 2] = 2;
                GameOver = true;
            }
            else if (Data[3, 2] != 1)
            {
                Data[3, 2] = Data[2, 2];
            }

            for (var row = 2; row > 0; row--)
            {
                Data[row, 0] = Data[row - 1, 0];
                Data[row, 1] = Data[row - 1, 1];
                Data[row, 2] = Data[row - 1, 2];
            }

            if (Data[0, 0] == 0 && Data[0, 1] == 0 && Data[0, 2] == 0)
            {
                var dataRow = GenerateDataRow();

                Data[0, 0] = dataRow[0];
                Data[0, 1] = dataRow[1];
                Data[0, 2] = dataRow[2];
            }
            else
            {
                Data[0, 0] = 0;
                Data[0, 1] = 0;
                Data[0, 2] = 0;
            }

            Finish--;
            Score = GameOver ? Score - 1 : Score + 1;
            AddHistory();
        }

        private void AddHistory()
        {
            gameHistory.Add(new GameHistory
            {
                Data = (int[,])Data.Clone(),
                GameOver = GameOver,
                Score = Score,
                Finish = Finish
            });
        }
    }
}
