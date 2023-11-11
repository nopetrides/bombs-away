using Bang.StateMachines;

namespace BombsAway.StateMachines.Gameplay
{
    public class CameraManager : StateMachine
    {
        public CameraManager()
        {
            State(Level);
        }

        private IEnumerator<Wait> Level()
        {
            
            yield return Wait.NextFrame;
        }
    }
}
