namespace Wpf2048.Model
{
    class FindAvailableCommand : IGameCommand {

        public GameCommandType Available { get; private set; }

        public void Execute(IGameField field) {

            GameCommandType type = 0;

            for (int j = 0; j < field.Matrix.GetLength(1); j++)
            {
                var prev = -1;

                for (int i = 0; i < field.Matrix.GetLength(0); i++) {
                    var current = field.Matrix[i, j];

                    if (prev == current && prev > 0) {
                        type = type | GameCommandType.Left | GameCommandType.Right;
                    }

                    if (current != 0 && prev == 0)
                        type = type | GameCommandType.Left;

                    if (current == 0 && prev > 0)
                        type = type | GameCommandType.Right;

                    prev = current;
                }

                if (type == (GameCommandType) 30) {
                    Available = type;
                    return;
                }
            }

            for (int i = 0; i < field.Matrix.GetLength(0); i++)
            {
                var prev = -1;

                for (int j = 0; j < field.Matrix.GetLength(1); j++)
                {
                    var current = field.Matrix[i, j];

                    if (prev == current && prev > 0) {
                        type = type | GameCommandType.Up | GameCommandType.Down;
                    }

                    if (current != 0 && prev == 0)
                        type = type | GameCommandType.Up;

                    if (current == 0 && prev > 0)
                        type = type | GameCommandType.Down;

                    prev = current;
                }

                if (type == (GameCommandType) 30) {
                    Available = type;
                    return;
                }
            }

            Available = type;
        }
    }
}