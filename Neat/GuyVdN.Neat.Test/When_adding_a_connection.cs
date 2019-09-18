using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace GuyVdN.Neat.Test
{
    public class When_adding_a_connection
    {
        [Test]
        public void The_innovation_number_should_be_correct()
        {
            var innovationHistory = new InnovationHistory();
            var genome = new Genome(3, 1, innovationHistory);

            genome.Connections.Count().ShouldBe(0);

            AddConnection(genome, 1, 4).InnovationNr.ShouldBe(1);
            AddConnection(genome, 2, 4).InnovationNr.ShouldBe(2);
            AddConnection(genome, 3, 4).InnovationNr.ShouldBe(3);
        }

        private static Connection AddConnection(Genome genome, int from, int to)
        {
            return genome.AddConnection(genome.Nodes.Single(n => n.Number == from), genome.Nodes.Single(n => n.Number == to));
        }
    }
}