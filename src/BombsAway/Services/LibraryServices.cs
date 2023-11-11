using Murder;
using BombsAway.Assets;
using Murder.Utilities;
using Bang;
using System.Numerics;
using Murder.Assets;

namespace BombsAway.Services
{
    public static class LibraryServices
    {
        public static LibraryAsset GetLibrary()
        {
            return Game.Data.GetAsset<LibraryAsset>(BombsAwayGame.Profile.Library);
        }

        internal static void Explode(int explosionSize, World world, Vector2 position)
        {
            var explosion = Game.Data.GetPrefab(GetLibrary().Explosions[explosionSize]).CreateAndFetch(world);
            explosion.SetGlobalPosition(position);
        }

        internal static PrefabAsset GetPauseMenuPrefab()
        {
            return Game.Data.GetAsset<PrefabAsset>(GetLibrary().PauseMenuPrefab);
        }
    }
}
