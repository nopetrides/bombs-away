using Bang.Components;

namespace BombsAway.Components
{
    public readonly struct ScoreValueComponent : IComponent
    {
        public readonly int Value = 1;

        public ScoreValueComponent(int value) 
        {  
            Value = value; 
        }
    }
}
