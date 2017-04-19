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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf2048.NeuralNetwork;

namespace Wpf2048 {
    /// <summary>
    /// Interaction logic for FlappyWindow.xaml
    /// </summary>
    public partial class FlappyWindow : Window {
        private Neuroevolution neuvol;
        private Game game;
        private int Fps = 60;

        private readonly Images images;

        public bool ApplicaitonRunning = true;

        public FlappyWindow() {
            InitializeComponent();

            images = new Images() {
                Background = new Image() {Source = new BitmapImage(new Uri(@"Images\background.png", UriKind.Relative)) },
                Bird = new Image() {Source = new BitmapImage(new Uri(@"Images\bird.png", UriKind.Relative))},
                Pipebottom = new Image() {Source = new BitmapImage(new Uri(@"Images\pipebottom.png", UriKind.Relative))},
                Pipetop = new Image() {Source = new BitmapImage(new Uri(@"Images\pipetop.png", UriKind.Relative))},
                Flappy = new Image() {Source = new BitmapImage(new Uri(@"Images\flappy.png", UriKind.Relative))},
            };
        }
        
        private void FlappyWindow_OnLoaded(object sender, RoutedEventArgs e) {
            neuvol = new Neuroevolution(new NeuralOptions() {
                Population = 50,
                Input = 2,
                Hiddens = new List<int>() {2},
                Output = 1
            });
            game = new Game(neuvol, Canvas, images) {Fps = 60};
            game.Start();
            game.Update();
            game.Display();

            new Thread(OnUpdate).Start();

        }

        private void OnUpdate() {

            while (ApplicaitonRunning) {
                Dispatcher.BeginInvoke(new Action(() => {
                    if (game != null)
                    {
                        game.Update();
                        game.Display();
                    }
                }));

                Thread.Sleep(1000/Fps);
            }

        }

        private void OnMaxClick(object sender, RoutedEventArgs e) {
            Fps = 2;
        }

        private void OnClosing(object sender, CancelEventArgs e) {
            ApplicaitonRunning = false;
        }

        private void Onx5Click(object sender, RoutedEventArgs e) {
            Fps = 60*5;
        }
        private void Onx3Click(object sender, RoutedEventArgs e) {
            Fps = 60*3;
        }
        private void Onx2Click(object sender, RoutedEventArgs e) {
            Fps = 60*2;
        }
        private void Onx1Click(object sender, RoutedEventArgs e) {
            Fps = 60;
        }

        private void RestartClick(object sender, RoutedEventArgs e) {
            if (game != null) {

                game.Display();
            }
        }
    }


    class Bird {
        public double X;
        public double Y;
        public double Width;
        public double Height;

        public bool Alive;


        public double Gravity;
        public double Velocity;

        public int Jump;

        public Bird() {
            this.X = 80;
            this.Y = 250;
            this.Width = 40;
            this.Height = 30;

            this.Alive = true;
            this.Gravity = 0;
            this.Velocity = 0.3;
            this.Jump = -6;
        }

        public void Init() {

        }

        public void Flap() {
            this.Gravity = this.Jump;
        }

        public void Update() {
            this.Gravity += this.Velocity;
            this.Y += this.Gravity;
        }

        public bool IsDead(double heightP, List<Pipe> pipes) {
            if (this.Y >= heightP || this.Y + this.Height <= 0) {
                return true;
            }
            foreach(var pipe in pipes)
            {
                if (!(
                    this.X > pipe.X + pipe.Width ||
                    this.X + this.Width < pipe.X ||
                    this.Y > pipe.Y + pipe.Height ||
                    this.Y + this.Height < pipe.Y
                )) {
                    return true;
                }
            }

            return false;
        }
    }

    class Pipe {
        public double X;
        public double Y;
        public double Width;
        public double Height;
        public int Speed;

        public bool Alive;

        public Pipe() {
            this.X = 0;
            this.Y = 0;
            this.Width = 50;
            this.Height = 40;
            this.Speed = 3;
        }

        public void Update() {
            this.X -= this.Speed;
        }

        public bool IsOut() {
            if (this.X + this.Width < 0)
                return true;
            return false;
        }
    }

    class Game {
        private readonly Neuroevolution neuroevolution;
        private readonly Canvas canvas;
        public List<Pipe> Pipes = new List<Pipe>();
        public List<Bird> Birds = new List<Bird>();
        public List<Network> Gen = new List<Network>();

        public int Score;
        public int MaxScore;
        public double Width;
        public double Height;

        public int SpawnInterval;
        public int Interval;
        public int Alives;
        public int Generation;
        public double BackgroundSpeed;
        public double Backgroundx;

        public int Fps = 60;

        private readonly Random random = new Random();

        private readonly Images images;

        public Game(Neuroevolution neuroevolution, Canvas canvas, Images images) {
            this.neuroevolution = neuroevolution;
            this.canvas = canvas;
            this.images = images;
            this.Score = 0;
            this.Width = this.canvas.ActualWidth;
            this.Height = this.canvas.ActualHeight;
            this.SpawnInterval = 90;
            this.Interval = 0;
            this.Alives = 0;
            this.Generation = 0;
            this.BackgroundSpeed = 0.5;
            this.Backgroundx = 0;
            this.MaxScore = 0;
        }

        public void Start()
        {
            this.Interval = 0;
            this.Score = 0;
            this.Pipes.Clear();
            this.Birds.Clear();

            this.Gen = neuroevolution.NextGeneration();

            foreach(var network in Gen) {
                this.Birds.Add(new Bird());
            }

            this.Generation++;
            this.Alives = this.Birds.Count;
        }

        public bool IsItEnd() {
            return !Birds.Any(b => b.Alive);
        }

        public void Update()
        {
            this.Backgroundx += this.BackgroundSpeed;
            var nextHoll = 0d;
            if (this.Birds.Count > 0)
            {
                for (var i = 0; i < this.Pipes.Count; i += 2)
                {
                    if (this.Pipes[i].X + this.Pipes[i].Width > this.Birds[0].X)
                    {
                        nextHoll =(double) this.Pipes[i].Height / this.Height;
                        break;
                    }
                }
            }

            for(int i = 0; i < Birds.Count; i++) {
                if (this.Birds[i].Alive) {

                    var inputs = new List<double> {(double) (this.Birds[i].Y/this.Height), (double) nextHoll};

                    var res = this.Gen[i].Compute(inputs).Last();
                    if (res > 0.5)
                    {
                        this.Birds[i].Flap();
                    }

                    this.Birds[i].Update();
                    if (this.Birds[i].IsDead(this.Height, this.Pipes))
                    {
                        this.Birds[i].Alive = false;
                        this.Alives--;
                        neuroevolution.NetworkScore(this.Gen[i], this.Score);
                        if (this.IsItEnd())
                        {
                            this.Start();
                        }
                    }
                }
            }

            for(var i = 0; i < this.Pipes.Count; i++) {
                this.Pipes[i].Update();
                if (this.Pipes[i].IsOut()) {
                    this.Pipes.RemoveAt(i);
                    i--;
                }
            }

            if (this.Interval == 0)
            {
                var deltaBord = 50;
                var pipeHoll = 120;
                var hollPosition = Math.Round(random.NextDouble() * (this.Height - deltaBord * 2 - pipeHoll)) + deltaBord;
                this.Pipes.Add(new Pipe() {X = this.Width, Y = 0, Height = hollPosition});
                this.Pipes.Add(new Pipe {X = this.Width, Y = hollPosition + pipeHoll, Height = this.Height});
            }

            this.Interval++;
            if (this.Interval == this.SpawnInterval)
            {
                this.Interval = 0;
            }

            this.Score++;
            this.MaxScore = (this.Score > this.MaxScore) ? this.Score : this.MaxScore;

        }

        private void DrawImage(Image image, double x, double y) {

            canvas.Children.Add(image);
            Canvas.SetTop(image, y);
            Canvas.SetLeft(image, x);
        }

        private void DrawImage(Image image, double x, double y, double w, double h) {
            image.Width = w;
            image.Height = h;
            canvas.Children.Add(image);
            Canvas.SetTop(image, y);
            Canvas.SetLeft(image, x);
        }

        public void Display() {
            this.canvas.Children.Clear();

            for (var i = 0; i < Math.Ceiling(this.Width / images.Background.Width) + 1; i++) {
                DrawImage(images.Background, i*images.Background.Width - Math.Floor(this.Backgroundx%images.Background.Width), 0);
            }

            for(int i = 0; i < Pipes.Count; i++) {

                var pipetop = images.Pipetop;
                if (i % 2 == 0) {
                    DrawImage(pipetop, this.Pipes[i].X, this.Pipes[i].Y + this.Pipes[i].Height - pipetop.Source.Height, this.Pipes[i].Width, pipetop.Source.Height);
                }
                else {
                    var pipebottom = images.Pipebottom;

                    DrawImage(pipebottom, this.Pipes[i].X, this.Pipes[i].Y, this.Pipes[i].Width, pipetop.Source.Height);
                }
            }
            

            for(int i = 0; i < Birds.Count; i++) {
                if (this.Birds[i].Alive)
                {
                    var image = images.Bird;
                    image.RenderTransform = new TransformGroup() {Children = new TransformCollection() {
                        new TranslateTransform(this.Birds[i].X + this.Birds[i].Width / 2, this.Birds[i].Y + this.Birds[i].Height / 2),
                        new RotateTransform(Math.PI / 2 * this.Birds[i].Gravity / 20)
                    }};

                    DrawImage(image, -this.Birds[i].Width/2, -this.Birds[i].Height/2, this.Birds[i].Width, this.Birds[i].Height);
                }
            }

            var scoreControls = new StackPanel();

            canvas.Children.Add(scoreControls);

            scoreControls.Children.Add(new TextBlock() { Text = "Score : " + this.Score });
            scoreControls.Children.Add(new TextBlock() { Text = "Max Score : " + this.MaxScore });
            scoreControls.Children.Add(new TextBlock() { Text = "Generation : " + this.Generation });
            scoreControls.Children.Add(new TextBlock() { Text = "Alive : " + this.Alives + " / " + neuroevolution.Options.Population });

        }

    }

    class Images
    {
        public Image Background {
            get { return Clone(background); }
            set { background = value; }
        }

        public Image Pipetop {
            get { return Clone(pipetop); }
            set { pipetop = value; }
        }
        public Image Pipebottom {
            get { return Clone(pipebottom); }
            set { pipebottom = value; }
        }
        public Image Bird {
            get { return Clone(bird); }
            set { bird = value; }
        }
        public Image Flappy {
            get { return Clone(flappy); }
            set { flappy = value; }
        }

        public Image Clone(Image image) {
            return new Image() {Source = image.Source};
        }

        public Image background;
        public Image pipetop;
        public Image pipebottom;
        public Image bird;
        public Image flappy;
    }

}

