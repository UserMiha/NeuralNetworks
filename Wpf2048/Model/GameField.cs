using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wpf2048.Model
{
    public interface IGameField {
        int[,] Matrix { get; }

        void Up();
        void Down();
        void Left();
        void Right();

        IGameField Clone();
    }

    class GameField : IGameField
    {
        public int[,] Matrix { get; set; }

        public void Up() {
            new UpCommand().Execute(this);
        }

        public void Down() {
            new DownCommand().Execute(this);
        }

        public void Left() {
            new LeftCommand().Execute(this);
        }

        public void Right() {
            new RightCommand().Execute(this);
        }

        public IGameField Clone() {
            return new GameField(0, 0) {Matrix = Matrix.Clone() as int[,]};
        }

        public GameField(int w, int h) {
            Matrix = new int[w, h];
        }

    }
}
