using Bang.Components;

namespace HelloMurder.Components
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
