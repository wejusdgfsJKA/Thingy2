using EventBus;
using HP;
namespace Player
{
    public class Player : HPComponent
    {
        protected override void Awake()
        {
            base.Awake();
            OnDeath.AddListener(() => EventBus<PlayerDeath>.Raise(new()));
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            GameManager.Player = transform;
        }
        protected void OnDisable()
        {
            GameManager.Player = null;
        }
    }
}