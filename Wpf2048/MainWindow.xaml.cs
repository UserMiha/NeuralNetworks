using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf2048.Model;

namespace Wpf2048
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private GameField GameField;

        public MainWindow()
        {
            InitializeComponent();
            
            GameField = new GameField(4, 4);

            GameField.Matrix[0, 2] = 2;
            GameField.Matrix[0, 3] = 2;

            GameField.Matrix[1, 0] = 2;
            GameField.Matrix[1, 1] = 2;
            GameField.Matrix[1, 2] = 2;
            GameField.Matrix[1, 3] = 2;

            GameField.Matrix[2, 0] = 2;
            GameField.Matrix[2, 1] = 0;
            GameField.Matrix[2, 2] = 0;
            GameField.Matrix[2, 3] = 2;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {
            Print();
        }

        private void UpCommandClick(object sender, RoutedEventArgs e) {
            new UpCommand().Execute(GameField);
            Print();
        }

        private void LeftCommandClick(object sender, RoutedEventArgs e)
        {
            new LeftCommand().Execute(GameField);
            Print();
        }

        private void RightCommandClick(object sender, RoutedEventArgs e)
        {
            new RightCommand().Execute(GameField);
            Print();
        }

        private void DownCommandClick(object sender, RoutedEventArgs e) {
            new DownCommand().Execute(GameField);
            Print();
        }

        private void AddNewDigit() {
            var addNewDigitCommand = new AddNewDigitCommand();
            addNewDigitCommand.Execute(GameField);
            if (addNewDigitCommand.IsFullField) {
                GameField = new GameField(GameField.Matrix.GetLength(0), GameField.Matrix.GetLength(1));
            }
        }

        private void Print() {
            FieldPanel.Children.Clear();

            AddNewDigit();
            FindAvailableCommands();

            for (int j = 0; j < GameField.Matrix.GetLength(1); j++) {

                var row = new DockPanel() {HorizontalAlignment = HorizontalAlignment.Left};
                DockPanel.SetDock(row, Dock.Top);
                FieldPanel.Children.Add(row);

                for (int i = 0; i < GameField.Matrix.GetLength(0); i++) {
                    row.Children.Add(new Label() {
                        FontSize = 36,
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brushes.Black,
                        Foreground = Brushes.DarkBlue,
                        Width = 60,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Content = GameField.Matrix[i, j].ToString()
                    });
                }
            }

            var rowEnd = new DockPanel() { HorizontalAlignment = HorizontalAlignment.Left };
            DockPanel.SetDock(rowEnd, Dock.Top);
            FieldPanel.Children.Add(rowEnd);
        }

        private void FindAvailableCommands() {
            var command = new FindAvailableCommand();
            command.Execute(GameField);

            TopButton.IsEnabled = command.Available.HasFlag(GameCommandType.Up);
            DownButton.IsEnabled = command.Available.HasFlag(GameCommandType.Down);
            LeftButton.IsEnabled = command.Available.HasFlag(GameCommandType.Left);
            RightButton.IsEnabled = command.Available.HasFlag(GameCommandType.Right);
        }

        private void CreateNewGameCommandClick(object sender, RoutedEventArgs e) {
            GameField = new GameField(GameField.Matrix.GetLength(0), GameField.Matrix.GetLength(1));
            Print();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key) {
                case Key.Up:
                    UpCommandClick(sender, null);
                    break;
                case Key.Down:
                    DownCommandClick(sender, null);
                    break;
                case Key.Left:
                    LeftCommandClick(sender, null);
                    break;
                case Key.Right:
                    RightCommandClick(sender, null);
                    break;
            }
        }
    }
}
