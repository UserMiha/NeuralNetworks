using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf2048.Model;
using Wpf2048.NeuralNetwork;

namespace Wpf2048
{
    /// <summary>
    /// Interaction logic for Game2048Window.xaml
    /// </summary>
    public partial class Game2048Window : Window
    {
        private Neuroevolution neuvol;
        private Game2048 game;
        private int Fps = 100;

        private GameField gameField;

        public bool ApplicaitonRunning = true;

        public Game2048Window()
        {
            InitializeComponent();

            InitGameField();
        }

        private void InitGameField() {

            gameField = new GameField(4, 4);

            gameField.Matrix[0, 2] = 2;
            gameField.Matrix[0, 3] = 2;

            gameField.Matrix[1, 0] = 2;
            gameField.Matrix[1, 1] = 2;
            gameField.Matrix[1, 2] = 2;
            gameField.Matrix[1, 3] = 2;

            gameField.Matrix[2, 0] = 2;
            gameField.Matrix[2, 1] = 0;
            gameField.Matrix[2, 2] = 0;
            gameField.Matrix[2, 3] = 2;
        }

        private void Print(IGameField field) {
            FieldPanel.Children.Clear();

            for (int j = 0; j < field.Matrix.GetLength(1); j++)
            {
                var row = new DockPanel() { HorizontalAlignment = HorizontalAlignment.Left };
                DockPanel.SetDock(row, Dock.Top);
                FieldPanel.Children.Add(row);

                for (int i = 0; i < field.Matrix.GetLength(0); i++)
                {
                    row.Children.Add(new Label()
                    {
                        FontSize = 36,
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brushes.Black,
                        Foreground = Brushes.DarkBlue,
                        Width = 80,
                        Height= 80,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Content = field.Matrix[i, j].ToString()
                    });
                }
            }

            var rowEnd = new DockPanel() { HorizontalAlignment = HorizontalAlignment.Left };
            DockPanel.SetDock(rowEnd, Dock.Top);
            FieldPanel.Children.Add(rowEnd);
        }

        private void Print()
        {
            Print((gameField));
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e) {

            neuvol = new Neuroevolution(new NeuralOptions()
            {
                Population = 100,
                Input = 4*4,
                Hiddens = new List<int>() { 4 },
                Output = 4,

                MutationRange = 0.6d,
                MutationRate = 0.2,
                RandomBehaviour = 0.2d,
                Elitism = 0.5

        });
            game = new Game2048(neuvol, new GameField(4,4)) { Fps = 60 };
            game.Start();
            game.Update();
            Print();

            new Thread(OnUpdate).Start();

        }

        private void OnUpdate()
        {
            while (ApplicaitonRunning)
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    if (game != null)
                    {
                        game.Update();
                        if (oldMaxScoreSum != game.MaxScoreSum)
                            Print(game.BestStrategy.Field);
                        UpdateScoreLabels();
                    }
                }));

                Thread.Sleep(1000 / Fps);
            }

        }

        private int oldMaxScoreSum = 0;

        private void UpdateScoreLabels() {
            MaxScoreLabel.Content = game.MaxScore;
            MaxSumScoreLabel.Content = game.MaxScoreSum;
            AlivesLabel.Content = string.Format("{0}/{1}", game.Alives, game.Strategies.Count);
            GenerationLabel.Content = game.Generation;
        }

        private void Game2048Window_OnClosing(object sender, CancelEventArgs e) {
            ApplicaitonRunning = false;
        }

        private void RestartClick(object sender, RoutedEventArgs e) {
            if (game != null) {
                if(game.BestStrategy!=null)
                    Print(game.BestStrategy.Field);
            }
        }

        private void Onx1Click(object sender, RoutedEventArgs e) {
            Fps = 60;
        }

        private void Onx2Click(object sender, RoutedEventArgs e) {
            Fps = 60 * 2;
        }

        private void Onx3Click(object sender, RoutedEventArgs e) {
            Fps = 60 * 3;
        }

        private void Onx5Click(object sender, RoutedEventArgs e) {
            Fps = 60 * 5;
        }

        private void OnMaxClick(object sender, RoutedEventArgs e) {
            Fps = 400;
        }
    }

    class Strategy {

        public int Score;
        public int StepCount;

        /// <summary>
        /// сумма игрового поля
        /// </summary>
        public int ScoreSum;

        public readonly IGameField Field;
        public bool Alive = true;

        public Strategy(IGameField field) {
            this.Field = field;
        }

        public void AddDigit() {
            var addNewDigitCommand = new AddNewDigitCommand();
            addNewDigitCommand.Execute(Field);

            if (addNewDigitCommand.IsFullField) {
                throw new Exception("Нет свободного места");
            }
        }

        public List<double> GetInputSignal() {
            var max = 1d;
            var inputs = new List<double>();

            for(int i = 0; i < Field.Matrix.GetLength(0); i++) {
                for(int j = 0; j < Field.Matrix.GetLength(1); j++) {
                    var value = Field.Matrix[i, j];
                    if (value > max)
                        max = value;
                    inputs.Add(value);
                }
            }

            return inputs.Select(v => v / max).ToList();
        }

        private int lastIndex = 0;

        public void MakeStep(List<double> outSignal, Network currentGen) {
            var maxIndex = -1;
            var maxValue = 0d;

            var findAvailableCommand = new FindAvailableCommand();

            findAvailableCommand.Execute(Field);

            for (int j = 0; j < outSignal.Count; j++)
            {
                //if ((j == 0 && !findAvailableCommand.Available.HasFlag(GameCommandType.Up))) continue;
                //if ((j == 1 && !findAvailableCommand.Available.HasFlag(GameCommandType.Right))) continue;
                //if ((j == 2 && !findAvailableCommand.Available.HasFlag(GameCommandType.Down))) continue;
                //if ((j == 3 && !findAvailableCommand.Available.HasFlag(GameCommandType.Left))) continue;

                var currentValue = outSignal[j];
                if (currentValue > maxValue)
                {
                    maxIndex = j;
                    maxValue = currentValue;
                }
            }

            //if (maxIndex == -1 && findAvailableCommand.Available > 0) {
            //    maxIndex = 0;
            //}

            if (maxIndex == -1)
                maxIndex = 0;

            var penalty = 0;

            if ((maxIndex == 0 && !findAvailableCommand.Available.HasFlag(GameCommandType.Up))) penalty = 1;
            if ((maxIndex == 1 && !findAvailableCommand.Available.HasFlag(GameCommandType.Right))) penalty = 1;
            if ((maxIndex == 2 && !findAvailableCommand.Available.HasFlag(GameCommandType.Down))) penalty = 1;
            if ((maxIndex == 3 && !findAvailableCommand.Available.HasFlag(GameCommandType.Left))) penalty = 1;

            if(penalty>0)
                Alive = false;

            var desired = new List<double>() {0, 0, 0, 0};

            if (findAvailableCommand.Available.HasFlag(GameCommandType.Up))
                desired[0] = 0.95d;
            if (findAvailableCommand.Available.HasFlag(GameCommandType.Right))
                desired[1] = 0.95d;
            if (findAvailableCommand.Available.HasFlag(GameCommandType.Down))
                desired[2] = 0.95d;
            if (findAvailableCommand.Available.HasFlag(GameCommandType.Left))
                desired[3] = 0.95d;

            currentGen.Learn(desired, outSignal);

            //lastIndex = ++lastIndex%4;

            //switch (lastIndex) {
            //    case 0:
            //        field.Up();
            //        break;
            //    case 1:
            //        field.Right();
            //        break;
            //    case 2:
            //        field.Down();
            //        break;
            //    case 3:
            //        field.Left();
            //        break;
            //}

            switch (maxIndex) {
                case 0:
                    Field.Up();
                    break;
                case 1:
                    Field.Right();
                    break;
                case 2:
                    Field.Down();
                    break;
                case 3:
                    Field.Left();
                    break;
            }

            Update();
        }

        public bool isAlive() {
            if (!Alive)
                return false;

            var availableCommand = new AnyAvailableCommand();
            availableCommand.Execute(Field);

            return Alive = availableCommand.Available;
        }

        private void Update() {
            Alive = false;

            StepCount++;
            Score = 0;
            ScoreSum = 0;
            for (int i = 0; i < Field.Matrix.GetLength(0); i++) {
                for(int j = 0; j < Field.Matrix.GetLength(1); j++) {
                    var value = Field.Matrix[i, j];
                    if (value != 0) {
                        Score += (int) Math.Floor(value*(1d + Math.Log(value)));
                        ScoreSum += value;
                    }
                    else Alive = true;
                }
            }
        }
    }

    class Game2048
    {
        private readonly Neuroevolution neuroevolution;
        private readonly GameField gameField;

        public List<Strategy> Strategies= new List<Strategy>();
        public List<Network> Gen = new List<Network>();

        public int Score;
        public int MaxScore;
        public int MaxScoreSum;

        public int Alives;
        public int Generation;

        public int Fps = 60;

        public Strategy BestStrategy;

        public Game2048(Neuroevolution neuroevolution, GameField gameField)
        {
            this.neuroevolution = neuroevolution;
            this.gameField = gameField;
            this.Score = 0;
            this.Alives = 0;
            this.Generation = 0;
            this.MaxScore = 0;
            this.MaxScoreSum = 0;
        }

        public void Start()
        {
            this.Score = 0;
            this.Strategies.Clear();

            this.Gen = neuroevolution.NextGeneration();

            foreach(var network in Gen) {
                var b = new Strategy(gameField.Clone());
                this.Strategies.Add(b);
            }

            if (BestStrategy == null)
                BestStrategy = Strategies[0];

            this.Generation++;
            this.Alives = this.Strategies.Count;
        }

        public bool IsItEnd() {
            return !Strategies.Any(b => b.Alive);
        }

        public void Update() {

            for(int i = 0; i < Strategies.Count; i++) {
                var strategy = Strategies[i];

                if (strategy.isAlive()) {
                    var currentGen = Gen[i];
                    strategy.AddDigit();

                    var output = currentGen.Compute(strategy.GetInputSignal());
                    strategy.MakeStep(output, currentGen);

                    //this.maxScore = Math.Max(strategy.score, maxScore);
                    //this.maxScoreSum = Math.Max(strategy.scoreSum, maxScoreSum);

                    if (strategy.StepCount > MaxScore) {
                        MaxScore = strategy.StepCount;
                        MaxScoreSum = strategy.ScoreSum;
                        BestStrategy = strategy;
                    }

                    //neuroevolution.networkScore(this.gen[i], strategy.score);

                    if (!strategy.isAlive()) {
                        Alives--;
                        neuroevolution.NetworkScore(this.Gen[i], strategy.StepCount);
                        if (this.IsItEnd()) {
                            this.Start();
                        }
                    }
                }
            }
        }
    }
}
