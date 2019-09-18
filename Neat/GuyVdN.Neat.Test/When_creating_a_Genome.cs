//using System;
//using System.Linq;
//using NUnit.Framework;
//using Shouldly;

//namespace GuyVdN.Neat.Test
//{
//    [TestFixture]
//    public class When_Creating_a_Genome
//    {
//        [TestCase(0)]
//        [TestCase(-1)]
//        [TestCase(-10)]
//        public void It_should_not_allow_zero_or_less_as_numberOfInputNodes(int numberOfInputNodes)
//        {
//            void Act() => new Genome(numberOfInputNodes, 5);
            
//            var exception = Should.Throw<ArgumentOutOfRangeException>(Act);
//            exception.Message.ShouldBe("Value should be greater than zero.\r\nParameter name: numberOfInputNodes");
//        }

//        [TestCase(0)]
//        [TestCase(-1)]
//        [TestCase(-10)]
//        public void It_should_not_allow_zero_or_less_as_numberOfOutputNodes(int numberOfOutputNodes)
//        {
//            void Act() => new Genome(5, numberOfOutputNodes);
//            var exception = Should.Throw<ArgumentOutOfRangeException>(Act);
//            exception.Message.ShouldBe("Value should be greater than zero.\r\nParameter name: numberOfOutputNodes");
//        }

//        [TestCase(1)]
//        [TestCase(5)]
//        [TestCase(9)]
//        public void It_should_add_the_correct_number_of_input_Nodes(int numberOfInputNodes)
//        {
//            const int bias = 1;
//            const int numberOfOutputNodes = 1;
//            var genome = new Genome(numberOfInputNodes, numberOfOutputNodes);
//            genome.Nodes.Count().ShouldBe(numberOfInputNodes + numberOfOutputNodes + bias);
//        }

//        [TestCase(1)]
//        [TestCase(5)]
//        [TestCase(9)]
//        public void It_should_add_the_correct_number_of_output_Nodes(int numberOfOutputNodes)
//        {
//            const int bias = 1;
//            const int numberOfInputNodes = 1;
//            var genome = new Genome(numberOfInputNodes, numberOfOutputNodes);
//            genome.Nodes.Count().ShouldBe(numberOfInputNodes + numberOfOutputNodes + bias);
//        }

//        [Test]
//        public void It_should_add_the_Nodes_in_the_correct_layer()
//        {
//            const int bias = 1;
//            const int numberOfInputNodes = 2;
//            const int numberOfOutputNodes = 3;

//            var genome = new Genome(numberOfInputNodes, numberOfOutputNodes);
//            genome.Nodes.Count(x => x.Layer == 0).ShouldBe(numberOfInputNodes + bias);
//            genome.Nodes.Count(x => x.Layer == 1).ShouldBe(numberOfOutputNodes);
//        }

//        [Test]
//        public void It_should_add_a_bias_Node_to_the_input_layer()
//        {
//            var genome = new Genome(1, 1);
//            genome.Nodes.Count().ShouldBe(3);
//            genome.Nodes.ShouldContain(Node.Bias);
//            Node.Bias.Layer.ShouldBe(0);
//        }

//        [Test]
//        public void It_should_mutate_once()
//        {
//            // TODO: not sure how to test this yet
//        }

//        [Test]
//        public void It_should_add_one_connection()
//        {
//            const int numberOfInputNodes = 2;
//            const int numberOfOutputNodes = 3;

//            var genome = new Genome(numberOfInputNodes, numberOfOutputNodes);
//            genome.Connections.Count().ShouldBe(1);
//        }
//    }
//}
