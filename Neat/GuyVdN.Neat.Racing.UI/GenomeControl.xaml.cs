using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GuyVdN.Neat;

namespace Neat
{
    /// <summary>
    /// Interaction logic for GenomeControl.xaml
    /// </summary>
    public partial class GenomeControl : UserControl
    {
        public GenomeControl()
        {
            InitializeComponent();
        }

        public void DrawGenome(Genome genome)
        {
            var maxNumberOfNodesOnLayer = genome.Nodes.GroupBy(x => x.Layer).Select(x => x.Count()).OrderByDescending(x => x).First();
            var circlePositions = new Dictionary<int, Tuple<int, int>>();

            GenomeCanvas.Children.Clear();
            GenomeCanvas.Width = genome.Layers * 150;
            GenomeCanvas.Height = maxNumberOfNodesOnLayer * 150;

            for (var layer = 1; layer <= genome.Layers; layer++)
            {
                var nodesInLayer = genome.Nodes.Where(x => x.Layer == layer).ToArray();
                var firstTop = (int)((GenomeCanvas.Height - nodesInLayer.Length * 150) / 2 + 50);

                for (var n = 0; n < nodesInLayer.Length; n++)
                {
                    var node = nodesInLayer[n];
                    var nodeLeft = 150 * layer - 100;
                    var nodeTop = 150 * n + firstTop;
                    circlePositions.Add(node.Number, new Tuple<int, int>(nodeLeft, nodeTop));

                    var circle = new Ellipse
                    {
                        Width = 50,
                        Height = 50,
                        Fill = new SolidColorBrush(Colors.White),
                        Stroke = new SolidColorBrush(Colors.Black)
                    };

                    Canvas.SetLeft(circle, nodeLeft);
                    Canvas.SetTop(circle, nodeTop);
                    Panel.SetZIndex(circle, 1);
                    GenomeCanvas.Children.Add(circle);

                    var text = new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = node.Number.ToString(),
                        FontSize = 24
                    };

                    var border = new Border
                    {
                        Width = 50,
                        Height = 50,
                        Child = text
                    };

                    Canvas.SetLeft(border, nodeLeft);
                    Canvas.SetTop(border, nodeTop);
                    Panel.SetZIndex(border, 2);
                    GenomeCanvas.Children.Add(border);
                    
                }
            }

            foreach (var connection in genome.Connections)
            {
                var (x1, y1) = circlePositions[connection.From.Number];
                var (x2, y2) = circlePositions[connection.To.Number];

                var offset = 18.38;
                var line = new Line
                {
                    X1 = x1 + 25,
                    Y1 = y1 + 25,
                    X2 = x2 + 25,
                    Y2 = y2 + 25,
                    Stroke = connection.IsEnabled ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Red)
                };

                Panel.SetZIndex(line, 0);
                GenomeCanvas.Children.Add(line);
            }
        }
    }
}
