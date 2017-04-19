namespace Wpf2048.NeuralNetwork
{
    internal class Genom
    {
        public readonly NetworkSaveData Network;
        public readonly int Score;

        public Genom(int score, NetworkSaveData network) {
            Score = score;
            Network = network;
        }

        public Genom Clone() {
            return new Genom(Score, Network.Clone());
        }
    }
}