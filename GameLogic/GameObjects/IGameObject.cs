namespace BubbleGame.GameLogic.GameObjects
{
    using ValueTypes;

    public interface IGameObject
    {
        public Point Location { get; set; }
    }
}