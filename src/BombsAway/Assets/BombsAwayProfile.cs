using BombsAway.Attributes;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Sounds;

namespace BombsAway.Assets
{
    public class BombsAwayProfile : GameProfile
    {
        [GameAssetId(typeof(LibraryAsset))]
        public readonly Guid Library;
        
        [FmodId(FmodIdKind.Bus)]
        [Tooltip("This is the bus in fmod that translates to the music setting.")]
        public readonly SoundEventId MusicBus;

        [FmodId(FmodIdKind.Bus)]
        [Tooltip("This is the bus in fmod that translates to the sound setting.")]
        public readonly SoundEventId SoundBus;
        
    }
}
