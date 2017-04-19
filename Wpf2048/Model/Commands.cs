using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wpf2048.Model
{
    public interface IGameCommand
    {
        void Execute(IGameField field);
    }

    class UpCommand : IGameCommand
    {
        public void Execute(IGameField field)
        {

            for (int i = 0; i < field.Matrix.GetLength(0); i++)
            {

                var list = new List<int>();

                for (int j = 0; j < field.Matrix.GetLength(1); j++)
                {
                    var current = field.Matrix[i, j];

                    if (current != 0)
                    {
                        list.Add(current);
                        field.Matrix[i, j] = 0;
                    }
                }

                var prev = -1;
                var index = 0;
                for (int j = 0; j < list.Count; j++)
                {
                    var current = list[j];

                    if (prev == current && prev > 0)
                    {
                        field.Matrix[i, index - 1] = prev + current;
                        prev = -1;
                    }
                    else
                    {
                        field.Matrix[i, index++] = current;
                        prev = current;
                    }

                }
            }

        }
    }

    class DownCommand : IGameCommand
    {
        public void Execute(IGameField field)
        {
            for (int i = 0; i < field.Matrix.GetLength(0); i++)
            {
                var list = new List<int>();

                var height = field.Matrix.GetLength(1);

                for (int j = height - 1; j >= 0; j--)
                {
                    var current = field.Matrix[i, j];

                    if (current != 0)
                    {
                        list.Add(current);
                        field.Matrix[i, j] = 0;
                    }
                }

                var prev = -1;
                var index = height - 1;
                for (int j = 0; j < list.Count; j++)
                {
                    var current = list[j];

                    if (prev == current && prev > 0)
                    {
                        field.Matrix[i, index + 1] = prev + current;
                        prev = -1;
                    }
                    else
                    {
                        field.Matrix[i, index--] = current;
                        prev = current;
                    }

                }
            }

        }
    }

    class LeftCommand : IGameCommand
    {
        public void Execute(IGameField field)
        {
            for (int j = 0; j < field.Matrix.GetLength(1); j++)
            {
                var list = new List<int>();

                for(int i = 0; i < field.Matrix.GetLength(0); i++) {
                    var current = field.Matrix[i, j];

                    if (current != 0) {
                        list.Add(current);
                        field.Matrix[i, j] = 0;
                    }
                }

                var prev = -1;
                var index = 0;

                for(int k = 0; k < list.Count; k++) {
                    var current = list[k];

                    if (prev == current && prev > 0) {
                        field.Matrix[index - 1, j] = prev + current;
                        prev = -1;
                    }
                    else {
                        field.Matrix[index++, j] = current;
                        prev = current;
                    }
                }
            }
        }
    }

    class RightCommand : IGameCommand
    {
        public void Execute(IGameField field)
        {
            var width = field.Matrix.GetLength(0);

            for (int j = 0; j < field.Matrix.GetLength(1); j++)
            {
                var list = new List<int>();

                for (int i = width - 1; i >= 0 ; i--)
                {
                    var current = field.Matrix[i, j];

                    if (current != 0)
                    {
                        list.Add(current);
                        field.Matrix[i, j] = 0;
                    }
                }

                var prev = -1;
                var index = width - 1;

                for (int k = 0; k < list.Count; k++)
                {
                    var current = list[k];

                    if (prev == current && prev > 0)
                    {
                        field.Matrix[index + 1, j] = prev + current;
                        prev = -1;
                    }
                    else
                    {
                        field.Matrix[index--, j] = current;
                        prev = current;
                    }
                }
            }
        }
    }
}
