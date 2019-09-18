using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace GuyVdN.Neat.Test
{
    public class When_adding_a_node
    {
        private Genome genome;
        private Node newNode;

        private static Connection AddConnection(Genome genome, int from, int to)
        {
            return genome.AddConnection(genome.Nodes.Single(n => n.Number == from), genome.Nodes.Single(n => n.Number == to));
        }

        [SetUp]
        public void SetUp()
        {
            Given();
            VerifySetup();
            When();
        }
        
        private void Given()
        {
            var innovationHistory = new InnovationHistory();
            genome = new Genome(3, 1, innovationHistory);
            
            AddConnection(genome, 1, 4).InnovationNr.ShouldBe(1);
            AddConnection(genome, 2, 4).InnovationNr.ShouldBe(2);
            AddConnection(genome, 3, 4).InnovationNr.ShouldBe(3);
        }

        private void VerifySetup()
        {
            genome.Layers.ShouldBe(2);
            genome.Nodes.Count().ShouldBe(5);
            genome.Connections.Count().ShouldBe(3);
        }

        private void When()
        {
            newNode = genome.MutateAddNode(genome.Nodes.Single(n => n.Number == 3), genome.Nodes.Single(n => n.Number == 4));
        }

        [Test]
        public void The_number_of_layers_in_the_genome_should_have_increased()
        {
            genome.Layers.ShouldBe(3);
        }

        [Test]
        public void The_layer_of_the_to_node_should_be_correct()
        {
            genome.Nodes.Single(n => n.Number == 4).Layer.ShouldBe(3);
        }

        [Test]
        public void The_number_of_nodes_in_the_genome_should_have_increased()
        {
            genome.Nodes.Count().ShouldBe(6);
        }

        [Test]
        public void The_number_of_connections_should_have_increased()
        {
            genome.Connections.Count().ShouldBe(5);
        }

        [Test]
        public void The_layer_of_the_new_node_should_be_correct()
        {
            newNode.Layer.ShouldBe(2);
        }

        [Test]
        public void The_innovation_numbers_of_the_new_connections_should_be_correct()
        {
            genome.Connections.Single(c => c.From.Number == 3 && c.To.Number == 5).InnovationNr.ShouldBe(4);
            genome.Connections.Single(c => c.From.Number == 5 && c.To.Number == 4).InnovationNr.ShouldBe(5);
        }

        [Test]
        public void The_original_connection_should_be_disabled()
        {
            genome.Connections.Single(c => c.From.Number == 3 && c.To.Number == 4).IsEnabled.ShouldBe(false);
        }

        [Test]
        public void The_weight_from_the_from_node_to_the_new_node_should_be_one()
        {
            genome.Connections.Single(c => c.From.Number == 3 && c.To.Number == 5).Weight.ShouldBe(1);
        }

        [Test]
        public void The_weight_from_the_new_node_to_the_to_node_should_be_the_same_as_the_old_connection()
        {
            var oldWeight = genome.Connections.Single(c => c.From.Number == 3 && c.To.Number == 4).Weight;
            genome.Connections.Single(c => c.From.Number == 5 && c.To.Number == 4).Weight.ShouldBe(oldWeight);
        }
    }
}