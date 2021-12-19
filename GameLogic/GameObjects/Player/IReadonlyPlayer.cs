namespace BubbleGame.GameLogic.GameObjects.Player
{
    using ValueTypes;

    public interface IReadonlyPlayer
    {
        public Point Location { get; }
        public Size Size { get; }
    }
}