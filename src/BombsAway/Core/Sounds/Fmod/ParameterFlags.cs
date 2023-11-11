
namespace BombsAway.Core.Sounds.Fmod
{
    [Flags]
    public enum ParameterFlags
    {
        None = 0,
        Readonly = 0x00000001,
        Automatic = 0x00000002,
        Global = 0x00000004,
        Discrete = 0x00000008,
        Labeled = 0x00000010,
    }
}
