namespace BubbleGame.GameLogic.GameObjects.Player
{
    using ValueTypes;

    internal class Player : IReadonlyPlayer, IGameObject
    {
        public Point Location { get; set; }
        public Size Size { get; set; }
    }
}