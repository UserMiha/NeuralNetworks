using System;
using System.Collections.Generic;

namespace Wpf2048.NeuralNetwork
{
    internal class NeuralOptions
    {
        private readonly Random random = new Random();

        public NeuralOptions() {
            Input = 1;
            Hiddens = new List<int> {1};
            Output = 1;
            RandomClamped = () => random.NextDouble(); //*2d - 1d;
            Activation = a => {
                var ap = -a / 1d;
                return 1d / (1d + Math.Exp(ap));
            };

            LearningRate = 2;
            Population = 50;
            Elitism = 0.2;
            RandomBehaviour = 0.2;
            MutationRate = 0.1;
            MutationRange = 0.5;
            Historic = 0;
            LowHistoric = false;
            ScoreSort = -1;
            NbChild = 1;
        }

        public Func<double, double> Activation { get; set; }
        public Func<double> RandomClamped { get; set; }

        public int Input { get; set; }
        public List<int> Hiddens { get; set; }
        public int Output { get; set; }

        public int Population { get; set; }

        public double Elitism { get; set; }
        public double RandomBehaviour { get; set; }
        public double MutationRate { get; set; }
        public double LearningRate { get; set; }
        public double MutationRange { get; set; }

        public int Historic { get; set; }
        public bool LowHistoric { get; set; }

        public int ScoreSort { get; set; }

        public int NbChild { get; set; }
    }
}