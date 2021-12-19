namespace BubbleGame.GameLogic.GameObjects.Bubble
{
    using System;
    using Player;
    using ValueTypes;

    internal class Bubble : IGameObject, IReadonlyBubble
    {
        public Point Location { get; set; }
        public float Radius { get; set; }
        public int Color { get; set; }

        public bool IntersectsWithPlayer(IReadonlyPlayer p)
        {
            var halfPlayerSize = new Size(p.Size.Width / 2, p.Size.Height / 2);
            var playerMiddle = new Point(p.Location.X + halfPlayerSize.Width, p.Location.Y -halfPlayerSize.Height);
            var bubbleDistance = new Point(
                Math.Abs(Location.X - playerMiddle.X),
                Math.Abs(Location.Y - playerMiddle.Y)
            );

            if (bubbleDistance.X > (halfPlayerSize.Width + Radius))
            {
                return false;
            }

            if (bubbleDistance.Y > (halfPlayerSize.Height + Radius))
            {
                return false;
            }

            if (bubbleDistance.X <= halfPlayerSize.Width)
            {
                return true;
            }

            if (bubbleDistance.Y <= halfPlayerSize.Height)
            {
                return true;
            }

            var bubbleDistSq = Math.Pow(bubbleDistance.X - halfPlayerSize.Width, 2) + Math.Pow(bubbleDistance.Y - halfPlayerSize.Height, 2);

            return bubbleDistSq <= Math.Pow(Radius, 2);
        }
    }
}