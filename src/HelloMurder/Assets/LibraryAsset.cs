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


        [Tooltip("This is the bounds that the road which the player will be driving will be displayed.")]
        public IntRectangle Bounds = Rectangle.Empty;


        public readonly SoundEventId UiNavigate;
        public readonly SoundEventId UiSelect;
    }
}
