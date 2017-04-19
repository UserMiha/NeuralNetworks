using System;
using System.Collections.Generic;

namespace Wpf2048.Model
{
    class AddNewDigitCommand : IGameCommand {
        public bool IsFullField { get; private set; }

        public void Execute(IGameField field) {
            var minimum = int.MaxValue;

            var places = new List<Tuple<int, int>>();

            for(int i = 0; i < field.Matrix.GetLength(0); i++) {
                for(int j = 0; j < field.Matrix.GetLength(1); j++) {
                    var value = field.Matrix[i, j];

                    if (value == 0) {
                        places.Add(Tuple.Create(i, j));
                    }
                    //else if (value < minimum) {
                    //    minimum = value;
                    //    if(minimum == 2)
                    //        break;
                    //}
                }
            }

            if (places.Count == 0) {
                IsFullField = true;
                return;
            }

            if (minimum == int.MaxValue)
                minimum = 2;

            var random = new Random();
            var x = random.Next(places.Count);

            field.Matrix[places[x].Item1, places[x].Item2] = minimum;
        }

    }
}