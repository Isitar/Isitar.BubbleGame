namespace Bubblegame.AvaloniaBubbleGame.Components
{
    using System;
    using System.Diagnostics;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Media;
    using BubbleGame.GameLogic;

    public class GameRenderControl : Control
    {
        private readonly Game game;


        public GameRenderControl() : this(new Game()) { }

        private GameRenderControl(Game game)
        {
            this.game = game;
            Focusable = true;
            game.TickPassed += (_, _) => VisualChanged();
            sw = Stopwatch.StartNew();
        }

        private readonly Stopwatch sw;
        private readonly long lastMs = 0;
        private const long TargetFPS = 60;
        private const long MaxMsElapsed = 1000 / TargetFPS;

        public void VisualChanged()
        {
            if (sw.ElapsedMilliseconds - lastMs >= MaxMsElapsed)
            {
                InvalidateVisual();
            }
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (game.GameState)
            {
                case GameState.Initialized:
                    game.Start();
                    VisualChanged();
                    break;
                case GameState.Started:
                    if (e.Key == Key.Left)
                    {
                        game.MoveLeft();
                        VisualChanged();
                    }

                    if (e.Key == Key.Right)
                    {
                        game.MoveRight();
                        VisualChanged();
                    }

                    break;
                case GameState.GameOver:
                    if (e.Key == Key.Enter)
                    {
                        game.Restart();
                        VisualChanged();
                    }

                    break;
                case GameState.Won:
                    if (e.Key == Key.Enter)
                    {
                        game.Restart();
                        VisualChanged();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawBackground(DrawingContext context)
        {
            context.FillRectangle(Brushes.DarkGray, new Rect(new Point(0, 0), Bounds.Size));
            ;
        }

        private void RenderStartScreen(DrawingContext context)
        {
            DrawBackground(context);
            context.DrawText(Brushes.White, new Point(0, Bounds.Height / 2), new FormattedText("To start the game press any key", Typeface.Default, 15, TextAlignment.Center, TextWrapping.Wrap, Bounds.Size));
        }

        private Color ColorFromIndex(int i)
        {
            return i switch
            {
                0 => Color.FromRgb(255, 0, 0),
                1 => Color.FromRgb(0, 255, 0),
                2 => Color.FromRgb(0, 0, 255),
                3 => Color.FromRgb(255, 0, 255),
                4 => Color.FromRgb(0, 255, 255),
                5 => Color.FromRgb(255, 255, 0),
                _ => Color.FromRgb(0, 0, 0),
            };
        }

        private void RenderNextColor(DrawingContext context)
        {
            var x = 5;
            for (int i = 0; i < game.LevelDefinition[game.CurrentLevel]; i++)
            {
                var color = ColorFromIndex(i);
                var currentCol = game.CurrentColor == i;
                var b = new SolidColorBrush(color, currentCol ? 1 : 0.2);
                var radius = currentCol ? 20 : 5;
                context.DrawGeometry(b, new Pen(b), new EllipseGeometry(new Rect(x, 5, radius, radius)));
                x += radius + 5;
            }

            context.DrawText(Brushes.White, new Point(x, 0), new FormattedText("Catch this", Typeface.Default, 15, TextAlignment.Left, TextWrapping.Wrap, DesiredSize));
        }

        private void RenderGameScreen(DrawingContext context)
        {
            DrawBackground(context);

            var yOffset = 20;
            var xSkew = Bounds.Width / game.Width;
            var ySkew = Bounds.Height / game.Height;

            xSkew = Math.Min(ySkew, xSkew);
            ySkew = xSkew;

            Width = xSkew * game.Width;
            Height = ySkew * game.Height;

            foreach (var bubble in game.Bubbles)
            {
                var b = new SolidColorBrush(ColorFromIndex(bubble.Color));
                context.DrawGeometry(b, new Pen(b), new EllipseGeometry(new Rect(bubble.Location.X * xSkew, bubble.Location.Y * ySkew - yOffset, 2 * bubble.Radius * xSkew, 2 * bubble.Radius * ySkew)));
            }


            var player = game.Player;
            var playerRect = new Rect(player.Location.X * xSkew, player.Location.Y * ySkew - yOffset, player.Size.Width * xSkew, player.Size.Height * ySkew);
            context.FillRectangle(Brushes.Salmon, playerRect);

            RenderNextColor(context);
        }

        private void RenderWinScreen(DrawingContext context)
        {
            context.FillRectangle(Brushes.DarkGray, new Rect(new Point(0, 0), Bounds.Size));
            context.DrawText(Brushes.White, new Point(0, Bounds.Height / 2), new FormattedText("you won", Typeface.Default, 15, TextAlignment.Center, TextWrapping.Wrap, Bounds.Size));
        }

        private void RenderGameOverScreen(DrawingContext context)
        {
            context.FillRectangle(Brushes.DarkGray, new Rect(new Point(0, 0), Bounds.Size));
            context.DrawText(Brushes.White, new Point(0, Bounds.Height / 2), new FormattedText("game over", Typeface.Default, 15, TextAlignment.Center, TextWrapping.Wrap, Bounds.Size));
        }

        public override void Render(DrawingContext context)
        {
            switch (game.GameState)
            {
                case GameState.Initialized:
                    RenderStartScreen(context);
                    break;
                case GameState.Started:
                    RenderGameScreen(context);
                    break;
                case GameState.GameOver:
                    RenderGameOverScreen(context);
                    break;
                case GameState.Won:
                    RenderWinScreen(context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}