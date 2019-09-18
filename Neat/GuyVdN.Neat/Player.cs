using System.IO;

namespace GuyVdN.Neat
{
    public class Player
    {
        private static int nextAvailableNumber = 1;

        public int Number { get; }
        public Genome Genome { get; }

        public Player(Genome genome)
        {
            Number = nextAvailableNumber++;
            Genome = genome;
        }

        public double[] Move(double[] input)
        {
            Age++;
            return Genome.FeedForward(input);
        }

        public int Age { get; private set; }

        public bool IsAlive { get; private set; } = true;

        public void Die()
        {
            IsAlive = false;
        }

        public double Score { private set; get; }

        public void SetScore(double score)
        {
            Score = score;
        }

        public double Fitness { private set; get; }

        public void SetFitness(double fitness)
        {
            Guard.GreaterThanOrEqualToZero(() => fitness);

            Fitness = fitness;
        }

        public override string ToString()
        {
            var writer = new StringWriter();
            writer.WriteLine($"Player IsAlive: {IsAlive} with an age of {Age} and a score of {Score}");
            writer.WriteLine("Player Genome: ");
            writer.Write(Genome);
            return writer.ToString();
        }

        public Player Clone()
        {
            return new Player(Genome.Clone());
        }

        public bool IsWinner { get; private set; }

        public void Win()
        {
            IsWinner = true;
        }
    }
}