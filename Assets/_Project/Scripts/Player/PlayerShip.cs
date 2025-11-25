namespace Player
{
    public class PlayerShip : Ship
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            GameManager.Player = this;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            GameManager.Player = null;
        }
    }
}