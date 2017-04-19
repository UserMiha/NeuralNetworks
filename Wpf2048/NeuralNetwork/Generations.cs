using System.Collections.Generic;

namespace Wpf2048.NeuralNetwork
{
    internal class Generations
    {
        private readonly NeuralOptions options;
        public Generation CurrentGeneration;

        public List<Generation> GenerationsList;

        public Generations(NeuralOptions options) {
            this.options = options;
            GenerationsList = new List<Generation>();
            CurrentGeneration = new Generation(options);
        }

        public List<NetworkSaveData> FirstGeneration() {
            var @out = new List<NetworkSaveData>();

            for (var i = 0; i < options.Population; i++) {
                var nn = new Network(options);
                nn.PerceptronGeneration(options.Input, options.Hiddens, options.Output);
                @out.Add(nn.GetSave());
            }

            GenerationsList.Add(new Generation(options));

            return @out;
        }

        public List<NetworkSaveData> NextGeneration() {
            if (GenerationsList.Count == 0) return new List<NetworkSaveData>();

            var gen = GenerationsList[GenerationsList.Count - 1].CreateNextGeneration();
            GenerationsList.Add(new Generation(options));

            return gen;
        }

        public void AddGenome(Genom genome) {
            if (GenerationsList.Count == 0)
                return;

            GenerationsList[GenerationsList.Count - 1].AddGenome(genome);
        }
    }
}