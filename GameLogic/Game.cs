namespace BubbleGame.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GameObjects.Bubble;
    using GameObjects.Player;
    using GameObjects.ValueTypes;

    public class Game
    {
        private readonly Player player;
        private readonly List<Bubble> bubbles;

        private const float FieldWidth = 200;
        private const float FieldHeight = 400;

        private const float PlayerMoveDist = 10;
        private const float PlayerWidth = 20;
        private const float PlayerHeight = 5;

        private const float BubbleRadius = 2;

        public event EventHandler TickPassed;

        private const int TargetUps = 60;
        private const double BubbleSpawnRate = 0.1;

        private readonly int[] levelDefinition =
        {
            2,
            3,
            4,
            5,
            6
        };

        public IReadOnlyList<int> LevelDefinition => levelDefinition;

        private int bubblesCaughtInCurrentLevel = 0;
        public int BubblesCaughtInCurrentLevel => bubblesCaughtInCurrentLevel;

        public IEnumerable<IReadonlyBubble> Bubbles => bubbles;
        public IReadonlyPlayer Player => player;
        public float Width { get; private set; }
        public float Height { get; private set; }
        public GameState GameState { get; private set; }
        public int CurrentColor { get; private set; }
        public int CurrentLevel { get; private set; }
        private readonly Random rnd = new Random();

        public void MoveRight()
        {
            MovePlayer(PlayerMoveDist);
        }

        public void MoveLeft()
        {
            MovePlayer(-PlayerMoveDist);
        }

        public void Start()
        {
            if (GameState.Initialized != GameState)
            {
                return;
            }

            InitializeField();
            GameState = GameState.Started;
            GameLoop();
        }

        public void Stop()
        {
            GameState = GameState.Initialized;
            InitializeField();
        }

        public void Restart()
        {
            Stop();
        }

        public Game()
        {
            bubbles = new List<Bubble>();
            player = new Player();

            Width = FieldWidth;
            Height = FieldHeight;
            GameState = GameState.Initialized;
        }

        private void InitializeField()
        {
            bubbles.Clear();
            player.Location = new Point(Width / 2 - PlayerWidth / 2, FieldHeight - 2 * PlayerHeight);
            player.Size = new Size(PlayerWidth, PlayerHeight);
            CurrentLevel = 0;
            bubblesCaughtInCurrentLevel = 0;
            CurrentColor = 0;
        }

        private void MovePlayer(float distance)
        {
            var newLocation = new Point(player.Location.X + distance, player.Location.Y);
            if (newLocation.X < 0 || newLocation.X > Width)
            {
                return;
            }

            player.Location = newLocation;
        }

        private async Task GameLoop()
        {
            var sw = new Stopwatch();
            const int targetMs = 1000 / TargetUps;

            while (GameState.Started == GameState)
            {
                sw.Restart();
                Tick();
                sw.Stop();
                if (sw.ElapsedMilliseconds < targetMs)
                {
                    await Task.Delay((int)(targetMs - sw.ElapsedMilliseconds));
                }
            }
        }

        private bool CheckNextLevel()
        {
            if (bubblesCaughtInCurrentLevel < levelDefinition[CurrentLevel])
            {
                CurrentColor++;
                return false;
            }

            bubbles.Clear();
            CurrentLevel++;
            CurrentColor = 0;
            bubblesCaughtInCurrentLevel = 0;
            return true;
        }

        private void SpawnBubble()
        {
            if (rnd.NextDouble() > BubbleSpawnRate)
            {
                return;
            }

            bubbles.Add(new Bubble
            {
                Color = rnd.Next(0, levelDefinition[CurrentLevel]),
                Location = new Point((float)rnd.NextDouble() * Width, 0),
                Radius = BubbleRadius,
            });
        }

        private void Tick()
        {
            for (int i = bubbles.Count - 1; i >= 0; i--)
            {
                var bubble = bubbles[i];
                if (bubble.IntersectsWithPlayer(player))
                {
                    if (bubble.Color == CurrentColor)
                    {
                        bubblesCaughtInCurrentLevel++;
                        bubbles.RemoveAt(i);
                        if (CheckNextLevel())
                        {
                            return;
                        }
                    }
                    else
                    {
                        GameState = GameState.GameOver;
                        return;
                    }
                }

                bubble.Location = new Point(bubble.Location.X, bubble.Location.Y + 2);

                if (bubble.Location.Y + bubble.Radius > Height)
                {
                    bubbles.RemoveAt(i);
                }
            }

            SpawnBubble();
            TickPassed?.Invoke(this, EventArgs.Empty);
        }
    }
}