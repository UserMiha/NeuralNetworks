using System.Collections.Generic;
using System.Linq;

namespace Wpf2048.NeuralNetwork
{
    internal class Network
    {
        private readonly NeuralOptions options;

        public readonly List<Layer> Layers;

        public Network(NeuralOptions options) {
            this.options = options;
            Layers = new List<Layer>();
        }

        public void PerceptronGeneration(int input, List<int> hiddens, int output) {
            var index = 0;
            var previousNeurons = 0;
            var layer = new Layer(index++, options);
            layer.Populate(input, previousNeurons);
            previousNeurons = input;
            Layers.Add(layer);
            //index++;

            for (var i = 0; i < hiddens.Count; i++) {
                var tmpLayer = new Layer(index, options);
                tmpLayer.Populate(hiddens[i], previousNeurons);
                previousNeurons = hiddens[i];
                Layers.Add(tmpLayer);
                index++;
            }

            var outLayer = new Layer(index, options);
            outLayer.Populate(output, previousNeurons);

            Layers.Add(outLayer);
        }

        public NetworkSaveData GetSave() {
            var datas = new NetworkSaveData();

            foreach (var layer in Layers) {
                datas.NeuronsCount.Add(layer.Neurons.Count);

                foreach (var layerNeuron in layer.Neurons) foreach (var layerNeuronWeight in layerNeuron.Weights) datas.Weights.Add(layerNeuronWeight);
            }

            return datas;
        }

        public void SetSave(NetworkSaveData save) {
            var previousNeurons = 0;
            var index = 0;
            var indexWeights = 0;
            Layers.Clear();

            for (var i = 0; i < save.NeuronsCount.Count; i++) {
                var layer = new Layer(index, options);
                var count = save.NeuronsCount[i];

                layer.Populate(count, previousNeurons);

                for (var j = 0; j < layer.Neurons.Count; j++)
                for (var k = 0; k < layer.Neurons[j].Weights.Count; k++) {
                    layer.Neurons[j].Weights[k] = save.Weights[indexWeights];
                    indexWeights++;
                }

                previousNeurons = count;
                index++;
                Layers.Add(layer);
            }
        }

        public void Learn(List<double> desired, List<double> outSignal) {
            var prevLayer = Layers[0];

            for (var i = 1; i < Layers.Count; i++) {
                var currentLayer = Layers[i];

                for (var j = 0; j < currentLayer.Neurons.Count; j++) {
                    //var sum = 0d;

                    //for (int k = 0; k < prevLayer.neurons.Count; k++)
                    //{
                    //    sum += prevLayer.neurons[k].Value * currentLayer.neurons[j].Weights[k];
                    //}

                    ////outputNeuron.error = sigmoid.derivative(outputNeuron.output) * (results[i] - outputNeuron.output);
                    ////outputNeuron.adjustWeights();

                    ////// then adjusts the hidden neurons' weights, based on their errors
                    ////hiddenNeuron1.error = sigmoid.derivative(hiddenNeuron1.output) * outputNeuron.error * outputNeuron.weights[0];

                    ////currentLayer.neurons[j].Value = options.activation(sum);

                    var error = desired[j] - outSignal[j];

                    //currentLayer.neurons[j].Value = options.activation(sum)*(error + 1);
                    currentLayer.Neurons[j].Value += currentLayer.Neurons[j].Value * error * options.LearningRate;
                }
                prevLayer = currentLayer;
            }
        }

        public List<double> Compute(List<double> inputs) {
            for (var i = 0; i < inputs.Count; i++) if (i < Layers[0].Neurons.Count) Layers[0].Neurons[i].Value = inputs[i];

            var prevLayer = Layers[0];
            for (var i = 1; i < Layers.Count; i++) {
                var currentLayer = Layers[i];

                for (var j = 0; j < currentLayer.Neurons.Count; j++) {
                    var sum = 0d;

                    for (var k = 0; k < prevLayer.Neurons.Count; k++) sum += prevLayer.Neurons[k].Value * currentLayer.Neurons[j].Weights[k];

                    currentLayer.Neurons[j].Value = options.Activation(sum);
                }
                prevLayer = currentLayer;
            }

            var @out = Layers.Last().Neurons.Select(neuron => neuron.Value).ToList();

            return @out;
        }
    }
}