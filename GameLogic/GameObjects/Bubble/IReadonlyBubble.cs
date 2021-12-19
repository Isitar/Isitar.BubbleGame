namespace BubbleGame.GameLogic.GameObjects.Bubble
{
    using ValueTypes;

    public interface IReadonlyBubble
    {
        public Point Location { get;  }
        public float Radius { get; }
        public int Color { get;  }
    }
}