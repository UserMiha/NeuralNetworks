namespace Wpf2048.Model
{
    class AnyAvailableCommand : IGameCommand {

        public bool Available { get; private set; }

        public void Execute(IGameField field) {
            Available = true;
            var existNoZero = false;

            for (int j = 0; j < field.Matrix.GetLength(1); j++)
            {
                var prev = -1;

                for (int i = 0; i < field.Matrix.GetLength(0); i++) {
                    var current = field.Matrix[i, j];

                    if (prev == current && prev > 0) {
                        return;
                    }

                    if (current != 0 && prev == 0)
                        return;

                    if (current == 0 && prev > 0)
                        return;

                    prev = current;
                }
            }

            for (int i = 0; i < field.Matrix.GetLength(0); i++)
            {
                var prev = -1;

                for (int j = 0; j < field.Matrix.GetLength(1); j++)
                {
                    var current = field.Matrix[i, j];

                    if (prev == current && prev > 0) {
                        return;
                    }

                    if (current != 0 && prev == 0) return;

                    if (current == 0 && prev > 0) return;

                    if (current != 0)
                        existNoZero = true;

                    prev = current;
                }
            }

            Available = !existNoZero;
        }
    }
}