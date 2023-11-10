using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Sounds;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace HelloMurder.Assets
{
    public class LibraryAsset : GameAsset
    {
        public override string EditorFolder => "#\uf02dLibraries";

        public override char Icon => '\uf02d';

        public override Vector4 EditorColor => "#FA5276".ToVector4Color();

        [GameAssetId<WorldAsset>]
        public Guid MainMenu = Guid.Empty;

        [GameAssetId<PrefabAsset>]
        public Guid PauseMenuPrefab = Guid.Empty;

        [GameAssetId(typeof(WorldAsset))]
        public Guid GameOver = Guid.Empty;

        [GameAssetId<PrefabAsset>]
        public Guid BombPrefab = Guid.Empty;

        [GameAssetId<PrefabAsset>]
        public Guid FlakPrefab = Guid.Empty;

        [GameAssetId<PrefabAsset>]
        public readonly ImmutableArray<Guid> Explosions = ImmutableArray<Guid>.Empty;

        [GameAssetId<SpriteAsset>]
        public Guid PlayerDeath = Guid.Empty;

        [GameAssetId<SpriteAsset>]
        public Guid SplashScreen = Guid.Empty;

        [GameAssetId<SpriteAsset>]
        public Guid GameOverScreen = Guid.Empty;

        [GameAssetId<PrefabAsset>]
        public Guid GameOverPaper = Guid.Empty;

        [GameAssetId<SpriteAsset>]
        public Guid MurderLogo = Guid.Empty;

        [GameAssetId<SpriteAsset>]
        public Guid FmodLogo = Guid.Empty;


        [Tooltip("This is the bounds of the gameplay area.")]
        public IntRectangle Bounds = Rectangle.Empty;

        // UI
        public readonly SoundEventId UiNavigate;
        public readonly SoundEventId UiSelect;
        // Intro
        public readonly SoundEventId RadioBlip;
        // Gameplay
        public readonly SoundEventId BombDrop;
        public readonly SoundEventId BombHit;
        public readonly SoundEventId BombMiss;
        public readonly SoundEventId FlakSpawn;
        public readonly SoundEventId FlakExplode;
        public readonly SoundEventId FlakWarning;
        public readonly SoundEventId PlayerDeathExplosions;
        public readonly SoundEventId PlayerFinalExplosion;
        public readonly SoundEventId Turboprop;
        public readonly ParameterId Roll;
        public readonly ParameterId Thrust;
        // Music
        public readonly SoundEventId MainMenuMusic;
        public readonly SoundEventId GameMusic;
        public readonly SoundEventId EndScreen;
    }
}
