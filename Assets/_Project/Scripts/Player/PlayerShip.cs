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
        public override void Tick(float deltaTime) { base.Tick(deltaTime); }
        public void Exit()
        {
            GameManager.EndMission();
        }
    }
}