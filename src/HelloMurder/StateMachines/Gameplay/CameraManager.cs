using Bang.StateMachines;

namespace HelloMurder.StateMachines.Gameplay
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
