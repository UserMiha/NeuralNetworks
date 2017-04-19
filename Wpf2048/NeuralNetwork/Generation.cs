using System;
using System.Collections.Generic;

namespace Wpf2048.NeuralNetwork
{
    internal class Generation
    {
        public readonly List<Genom> Genomes;
        private readonly NeuralOptions options;

        private readonly Random rnd = new Random();

        public Generation(NeuralOptions options) {
            this.options = options;
            Genomes = new List<Genom>();
        }

        public void AddGenome(Genom genome) {
            int i;
            for (i = 0; i < Genomes.Count; i++)
                if (options.ScoreSort < 0) {
                    if (genome.Score > Genomes[i].Score)
                        break;
                }
                else {
                    if (genome.Score < Genomes[i].Score)
                        break;
                }
            if (i == Genomes.Count && i != 0)
                i--;

            Genomes.Insert(i, genome);
        }

        public List<Genom> Breed(Genom g1, Genom g2, int nbChilds) {
            var datas = new List<Genom>();

            for (var nb = 0; nb < nbChilds; nb++) {
                var data = g1.Clone();

                for (var i = 0; i < g2.Network.Weights.Count; i++) if (rnd.NextDouble() <= 0.5) data.Network.Weights[i] = g2.Network.Weights[i];

                for (var i = 0; i < data.Network.Weights.Count; i++) {
                    var rndDouble = rnd.NextDouble();
                    if (rndDouble <= options.MutationRate) data.Network.Weights[i] += rndDouble * options.MutationRange * 2 - options.MutationRange;
                }

                datas.Add(data);
            }

            return datas;
        }

        public List<NetworkSaveData> CreateNextGeneration() {
            var nexts = new List<NetworkSaveData>();

            for (var i = 0; i < Math.Round(options.Elitism * options.Population); i++)
                if (nexts.Count < options.Population)
                    nexts.Add(Genomes[i].Network.Clone());

            for (var i = 0; i < Math.Round(options.RandomBehaviour * options.Population); i++) {
                var n = Genomes[0].Network.Clone();

                for (var k = 0; k < n.Weights.Count; k++)
                    n.Weights[k] = options.RandomClamped();

                if (nexts.Count < options.Population)
                    nexts.Add(n);
            }

            var max = 0;
            while (true) {
                for (var i = 0; i < max; i++) {
                    var childs = Breed(Genomes[i], Genomes[max], options.NbChild > 0 ? options.NbChild : 1);

                    foreach (var child in childs) {
                        nexts.Add(child.Network);
                        if (nexts.Count >= options.Population)
                            return nexts;
                    }
                }

                max++;
                if (max >= Genomes.Count - 1) max = 0;
            }
        }
    }
}