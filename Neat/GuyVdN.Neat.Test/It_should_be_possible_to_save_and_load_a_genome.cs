using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using GuyVdN.Neat.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace GuyVdN.Neat.Test
{
    [TestFixture]
    public class It_should_be_possible_to_save_and_load_a_genome
    {
        private Genome genome;
        private Genome loadedGenome;

        [OneTimeSetUp]
        public void Setup()
        {
            genome = new Genome(5, 3, new InnovationHistory());
            3.Times(_ =>
            {
                genome.MutateAddConnection();
                genome.MutateAddNode();
            });

            var fileName = $"C:/temp/genome_{DateTime.Now:yy_MM_dd_hh_mm_ss}.json";
            FileHandler.Save(genome, fileName);
            loadedGenome = FileHandler.Load(fileName);
            loadedGenome.ShouldBeSameAs(genome);
        }

        [Test]
        public void The_number_of_layers_should_be_the_same()
        {
            loadedGenome.Layers.ShouldBe(genome.Layers);
        }

        [Test]
        public void The_nodes_should_be_the_same()
        {
            genome.Nodes.ForEach(node =>
                loadedGenome.Nodes.ShouldContain(n => n.Number == node.Number)
            );
        }

        [Test]
        public void The_connections_should_be_the_same()
        {
            genome.Connections.ForEach(connection =>
                loadedGenome.Connections.ShouldContain(c =>
                    c.From.Number == connection.From.Number &&
                    c.To.Number == connection.To.Number &&
                    c.InnovationNr == connection.InnovationNr &&
                    Math.Abs(c.Weight - connection.Weight) < 0.01 &&
                    c.IsEnabled == connection.IsEnabled));
        }

        [Test]
        public void ToString_should_return_the_same()
        {
            loadedGenome.ToString().ShouldBe(genome.ToString());
        }
    }
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
    public class FileHandler
    {
        public class JsonGenome
        {
            public int NumberOfInputNodes { get; set; }
            public int NumberOfOutputNodes { get; set; }
            public int Layers { get; set; }
            public List<JsonConnection> Connections { get; set; }
            public List<JsonNode> Nodes { get; set; }

            public JsonGenome(Genome genome)
            {
                NumberOfInputNodes = genome.NumberOfInputNodes;
                NumberOfOutputNodes = genome.NumberOfOutputNodes;
                Layers = genome.Layers;
            }

            [JsonConstructor]
            public JsonGenome() { }

            public Genome ToGenome()
            {
                return new Genome(NumberOfInputNodes, NumberOfOutputNodes, new InnovationHistory());
            }
        }

        public class JsonConnection
        {
            public int From { get; set; }
            public int To { get; set; }
            public double Weight { get; set; }
            public bool IsEnabled { get; set; }
            public int InnovationNr { get; set; }
        }

        public class JsonNode
        {

        }

        public static void Save(Genome genome, string fileName)
        {
            using (var file = File.CreateText(fileName))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, new JsonGenome(genome));
            }
        }

        public static Genome Load(string fileName)
        {
            using (var file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();
                var jsonGenome = (JsonGenome)serializer.Deserialize(file, typeof(JsonGenome));
                return jsonGenome.ToGenome();
            }
        }
    }
}
