using System.Collections.Generic;
using System.Linq;

//https://xviniette.github.io/FlappyLearning/

namespace Wpf2048.NeuralNetwork
{
    internal class NetworkSaveData
    {
        public List<int> NeuronsCount = new List<int>();
        public List<double> Weights = new List<double>();

        public NetworkSaveData Clone() {
            return new NetworkSaveData {NeuronsCount = NeuronsCount.ToList(), Weights = Weights.ToList()};
        }
    }

    internal class Neuroevolution
    {
        public NeuralOptions Options;

        public Neuroevolution(NeuralOptions options) {
            this.Options = options;
            Generations = new Generations(options);
        }

        public Generations Generations { get; private set; }

        public void Restart() {
            Generations = new Generations(Options);
        }

        public void Set(NeuralOptions option) {
            Options = option;
        }

        public List<Network> NextGeneration() {
            List<NetworkSaveData> networks;

            if (Generations.GenerationsList.Count == 0) networks = Generations.FirstGeneration();
            else networks = Generations.NextGeneration();

            var nns = new List<Network>();

            foreach (var network in networks) {
                var nn = new Network(Options);
                nn.SetSave(network);
                nns.Add(nn);
            }

            if (Options.LowHistoric)
                if (Generations.GenerationsList.Count >= 2) {
                    var genomes = Generations.GenerationsList[Generations.GenerationsList.Count - 2].Genomes;

                    genomes.Clear();
                }

            if (Options.Historic != -1)
                if (Generations.GenerationsList.Count > Options.Historic + 1) Generations.GenerationsList.RemoveRange(0, Generations.GenerationsList.Count - (Options.Historic + 1));
            return nns;
        }

        public void NetworkScore(Network network, int score) {
            Generations.AddGenome(new Genom(score, network.GetSave()));
        }
    }
}