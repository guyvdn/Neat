using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace GuyVdN.Neat.Test
{
    [TestFixture]
    public class When_mating_nodes
    {
        private InnovationHistory innovationHistory;
        private Genome parent1;
        private Genome parent2;

        [SetUp]
        public void SetUp()
        {
            innovationHistory = new InnovationHistory();

            parent1 = new Genome(3, 1, innovationHistory);
            parent1.AddConnection(parent1.GetNode(1), parent1.GetNode(4)).InnovationNr.ShouldBe(1); 
            parent1.AddConnection(parent1.GetNode(2), parent1.GetNode(4)).InnovationNr.ShouldBe(2);
            parent1.AddConnection(parent1.GetNode(3), parent1.GetNode(4)).InnovationNr.ShouldBe(3); 
            parent1.MutateAddNode(parent1.GetNode(2), parent1.GetNode(4)); // InnovationNr 4 + 5

            parent2 = new Genome(3, 1, innovationHistory);
            parent2.AddConnection(parent2.GetNode(1), parent2.GetNode(4)).InnovationNr.ShouldBe(1);
            parent2.AddConnection(parent2.GetNode(2), parent2.GetNode(4)).InnovationNr.ShouldBe(2); 
            parent2.AddConnection(parent2.GetNode(3), parent2.GetNode(4)).InnovationNr.ShouldBe(3); 
            parent2.MutateAddNode(parent2.GetNode(2), parent2.GetNode(4)); // InnovationNr 4 + 5
            parent2.MutateAddNode(parent2.GetNode(5), parent2.GetNode(4)); // InnovationNr 6 + 7

            parent1.AddConnection(parent1.GetNode(1), parent1.GetNode(5)).InnovationNr.ShouldBe(8); 
            parent2.AddConnection(parent2.GetNode(3), parent2.GetNode(5)).InnovationNr.ShouldBe(9); 
            parent2.AddConnection(parent2.GetNode(1), parent2.GetNode(6)).InnovationNr.ShouldBe(10);
        }

        [Test]
        public void It_should_get_all_nodes_from_the_first_parent()
        {
            var baby = parent1.Mate(parent2);

            Console.WriteLine(parent1);
            Console.WriteLine(baby);

            baby.Nodes.Count().ShouldBe(parent1.Nodes.Count());
            
            baby.Layers.ShouldBe(parent1.Layers);
        }

        [Test]
        public void It_should_get_all_nodes_from_the_second_parent()
        {
            var baby = parent2.Mate(parent1);

            Console.WriteLine(parent2);
            Console.WriteLine(baby);

            baby.Nodes.Count().ShouldBe(parent2.Nodes.Count());
            
            baby.Layers.ShouldBe(parent2.Layers);
        }
    }
}