
namespace BombsAway.Core.Sounds.Fmod
{
    public readonly struct ParameterLabel
    {
        public readonly string Label;
        public readonly float Value;

        public ParameterLabel(string label, float value)
        {
            Label = label;
            Value = value;
        }
    }
}
