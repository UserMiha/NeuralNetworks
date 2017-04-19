using System.Collections.Generic;

namespace Wpf2048.NeuralNetwork
{
    internal class Layer
    {
        public readonly int Id;
        public readonly List<Neuron> Neurons;

        private readonly NeuralOptions options;

        public Layer(int id, NeuralOptions options) {
            this.Id = id;
            this.options = options;
            Neurons = new List<Neuron>();
        }

        public void Populate(int nbNeurons, double nbInputs) {
            Neurons.Clear();

            for (var i = 0; i < nbNeurons; i++) {
                var n = new Neuron(options);
                n.Populate(nbInputs);
                Neurons.Add(n);
            }
        }
    }
}