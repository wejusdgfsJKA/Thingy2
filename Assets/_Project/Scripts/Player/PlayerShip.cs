namespace Player
{
    public class PlayerShip : Unit
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
        public void Exit()
        {
            GameManager.EndMission();
        }
    }
}