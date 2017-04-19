using System.Collections.Generic;

namespace Wpf2048.NeuralNetwork
{
    internal class Neuron
    {
        private readonly NeuralOptions options;

        public Neuron(NeuralOptions options) {
            this.options = options;
            Weights = new List<double>();
        }

        public double Value { get; set; }
        public List<double> Weights { get; set; }

        public void Populate(double nb) {
            Weights.Clear();

            for (var i = 0; i < nb; i++) Weights.Add(options.RandomClamped());
        }
    }
}