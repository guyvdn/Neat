using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GuyVdN.Neat
{
    public class Population
    {
        private readonly InnovationHistory innovationHistory;
        private readonly int numberOfPlayers;
        private readonly int numberOfInputNodes;
        private readonly int numberOfOutputNodes;
        private readonly NewPlayerAction newPlayerAction;
        private readonly List<Player> players;

        public IEnumerable<Player> Players => players;

        public Population(int numberOfPlayers, int numberOfInputNodes, int numberOfOutputNodes, NewPlayerAction newPlayerAction)
        {
            innovationHistory = new InnovationHistory();
            this.numberOfPlayers = numberOfPlayers;
            this.numberOfInputNodes = numberOfInputNodes;
            this.numberOfOutputNodes = numberOfOutputNodes;
            this.newPlayerAction = newPlayerAction;
            players = CreatePlayers().ToList();
        }

        private IEnumerable<Player> CreatePlayers()
        {
            for (var i = 1; i <= numberOfPlayers; i++)
            {
                yield return CreateNewPlayer();
            }
        }

        private Player CreateNewPlayer()
        {
            var playerGenome = new Genome(numberOfInputNodes, numberOfOutputNodes, innovationHistory);
            newPlayerAction.Execute(playerGenome);
            return new Player(playerGenome);
        }

        public Func<int, double[]> GetPlayerInput { get; set; }

        public Func<Player, double> GetPlayerScore { get; set; }
        public Func<Player, double> GetPlayerFitness { get; set; }

        /// <summary>
        /// Return false when the player dies
        /// </summary>
        public Func<Player, double[], int> ProcessOutput { get; set; }
        public bool IsAlive => players.Any(p => p.IsAlive);
        public double BestScore => players.Max(p => p.Score);
        public Player BestPlayer => players.OrderByDescending(p => p.Score).First();
        public double BestFitness => players.Max(p => p.Fitness);

        public void MovePlayers()
        {
            foreach (var player in players.Where(p => p.IsAlive))
            {
                var input = GetPlayerInput(player.Number);
                var output = player.Move(input);

                var outputResult = ProcessOutput(player, output);
                if (outputResult == -1)
                    player.Die();
                else if (outputResult == 1)
                    player.Win();

                player.SetScore(GetPlayerScore(player));
                player.SetFitness(GetPlayerFitness(player));
            }
        }

        public override string ToString()
        {
            var writer = new StringWriter();
            writer.WriteLine($"Population of {numberOfPlayers} players");
            writer.WriteLine($"{players.Count(p => p.IsAlive)} players are alive");
            writer.WriteLine($"The best score is: {BestScore} and the best fitness is: {BestFitness}");
            writer.WriteLine("Player:");
            foreach (var player in players)
            {
                writer.Write(player);
                writer.WriteLine("----------------------------------------------------");
            }

            foreach (var speci in species)
            {
                writer.Write(speci);
                writer.WriteLine("----------------------------------------------------");
            }

            return writer.ToString();
        }

        public void Evolve()
        {
            var bestPlayer = players.OrderByDescending(p => p.Fitness).First();

            Speciate();
            Purify();

            var avgFitnessSum = species.Sum(s => s.AverageFitness);
            var babies = new List<Player>();
            babies.Add(bestPlayer.Clone());

            foreach (var speci in species)
            {
                babies.Add(speci.BestPlayer.Clone());
                var nrOfBabiesToAdd = Math.Floor(speci.AverageFitness / avgFitnessSum * numberOfPlayers) - 1;

                for (var i = 0; i < nrOfBabiesToAdd; i++)
                {
                    babies.Add(speci.GetBaby());
                }
            }

            if (babies.Count >= numberOfPlayers)
                babies = babies.Take(numberOfPlayers - 1).ToList();

            while (babies.Count < numberOfPlayers)
            {
                if (species.Any())
                {
                    var bestSpecies = species.OrderByDescending(s => s.BestFitness).First();
                    babies.Add(bestSpecies.GetBaby());
                }
                else
                {
                    babies.Add(CreateNewPlayer());
                }
            }

            players.Clear();
            players.AddRange(babies);
        }

        public int Staleness => species.Any() ? species.Min(s => s.Staleness) : 0;

        private void Purify()
        {
            species.ToList().ForEach(s => s.Purify());

            // If all species have a staleness > 20, only keep the two best species
            if (species.Min(s => s.Staleness) > 20)
            {
                species.OrderByDescending(s => s.BestFitness).Skip(2).ToList().ForEach(s => species.Remove(s));
            }

            var staleSpecies = species.Where(s => s.Staleness >= 20).ToList();
            staleSpecies.ForEach(s => species.Remove(s));

            var avgFitnessSum = species.Sum(s => s.AverageFitness);
            var badSpecies = species.Where(s => s.AverageFitness / avgFitnessSum * numberOfPlayers < 1).ToList();
            badSpecies.ForEach(s => species.Remove(s));
        }

        private readonly List<Species> species = new List<Species>();
        public IEnumerable<Species> Species => species;

        private void Speciate()
        {
            species.ForEach(s => s.Clear());

            foreach (var player in players)
            {
                var sameSpecies = species.FirstOrDefault(s => s.IsCompatible(player));
                if (sameSpecies != null)
                {
                    sameSpecies.Add(player);
                }
                else
                {
                    species.Add(new Species(player));
                }
            }

            // remove species that are empty
            var emptySpecies = species.Where(s => s.IsEmpty).ToList();
            emptySpecies.ForEach(s => species.Remove(s));
        }

        public Player GetPlayer(int playerNumber)
        {
            return players.Single(p => p.Number == playerNumber);
        }
    }
}