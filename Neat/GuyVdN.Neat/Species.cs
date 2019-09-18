using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GuyVdN.Neat.Extensions;

namespace GuyVdN.Neat
{
    public class Species
    {
        private readonly List<Player> players = new List<Player>();
        private Player Representative { get; }

        private int staleness;
        public double BestFitness { get; private set; }
        public int Staleness => players.Any() ? staleness : 200;
        public double AverageFitness => players.Average(p => p.Fitness);
        public Player BestPlayer => players.OrderByDescending(p => p.Fitness).First();

        public bool IsEmpty => !players.Any();

        public Species(Player player)
        {
            Representative = player;
            BestFitness = player.Fitness;
        }

        public void Clear()
        {
            players.Clear();
            staleness++;
        }

        public bool IsCompatible(Player player)
        {
            const int excessCoeff = 1;
            const double weightDiffCoeff = 0.5;
            const int compatibilityThreshold = 3;

            var excessAndDisjoint = GetExcessDisjoint(player.Genome, Representative.Genome);
            var averageWeightDiff = AverageWeightDiff(player.Genome, Representative.Genome);

            var largeGenomeNormalizer = player.Genome.Connections.Count() - 20;
            if (largeGenomeNormalizer < 1)
            {
                largeGenomeNormalizer = 1;
            }

            var compatibility = excessCoeff * excessAndDisjoint / largeGenomeNormalizer + weightDiffCoeff * averageWeightDiff; //compatibility formula
            return compatibilityThreshold > compatibility;
        }

        private static double GetExcessDisjoint(Genome genome1, Genome genome2)
        {
            var enumerable = genome1.Connections.Join(genome2.Connections, c1 => c1.InnovationNr, c2 => c2.InnovationNr, (_, __) => 0);
            return genome1.Connections.Count() + genome2.Connections.Count() - 2 * enumerable.Count();
        }

        private static double AverageWeightDiff(Genome genome1, Genome genome2)
        {
            var weights = genome1.Connections.Join(genome2.Connections, c1 => c1.InnovationNr, c2 => c2.InnovationNr, (c1, c2) => Math.Abs(c1.Weight - c2.Weight)).ToList();

            if (!weights.Any())
                return 100;

            return weights.Sum() / weights.Count;
        }

        public void Add(Player player)
        {
            players.Add(player);

            if (player.Fitness <= BestFitness)
                return;

            BestFitness = player.Fitness;
            staleness = 0;
        }

        public override string ToString()
        {
            var writer = new StringWriter();
            writer.WriteLine($"Species IsAlive: {players.Any(p => p.IsAlive)} with {players.Count} players.");
            writer.WriteLine($" a BestFitness {BestFitness} and a Staleness of {Staleness}");
            return writer.ToString();
        }

        public Player GetBaby()
        {
            const double chanceOfCrossover = 0.75;
            var babyGenome = GetRandom.Instance().Double(0, 1) <= chanceOfCrossover ? GetCrossover() : GetClone();
            babyGenome.Mutate();
            return new Player(babyGenome);
        }

        private Player GetRandomPlayer()
        {
            var fitnessSum = players.Sum(p => p.Fitness);
            var randomValue = GetRandom.Instance().Double(fitnessSum);
            var runningSum = 0d;

            foreach (var player in players)
            {
                runningSum += player.Fitness;
                if (runningSum > randomValue)
                    return player;
            }

            return players.Last();
        }

        private Genome GetCrossover()
        {
            var parent1 = GetRandomPlayer();
            var parent2 = GetRandomPlayer();

            return parent1.Fitness > parent2.Fitness
                ? parent1.Genome.Mate(parent2.Genome)
                : parent2.Genome.Mate(parent1.Genome);
        }

        private Genome GetClone()
        {
            var parent = GetRandomPlayer();
            return parent.Genome.Clone();
        }

        /// <summary>
        /// Remove bottom half of players
        /// </summary>
        public void Purify()
        {
            var playersToRemove = players.OrderBy(p => p.Fitness).Take(players.Count / 2);
            playersToRemove.ForEach(p => players.Remove(p));
        }
    }
}