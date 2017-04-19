using System;

namespace Wpf2048.Model
{
    [Flags]
    public enum GameCommandType {
        None = 0,
        Left = 2,
        Right = 4,
        Up = 8,
        Down = 16
    }
}