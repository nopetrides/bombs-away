using Murder;
using HelloMurder;
using HelloMurder.Assets;
using Murder.Utilities;
using Bang;
using System.Numerics;

namespace HelloMurder.Services
{
    public static class LibraryServices
    {
        public static LibraryAsset GetLibrary()
        {
            return Game.Data.GetAsset<LibraryAsset>(HelloMurderGame.Profile.Library);
        }

        internal static void Explode(int explosionSize, World world, Vector2 position)
        {
            var explosion = Game.Data.GetPrefab(GetLibrary().Explosions[explosionSize]).CreateAndFetch(world);
            explosion.SetGlobalPosition(position);

            //LDGameSoundPlayer.Instance.PlayEvent(GetLibrary().CarImpact, isLoop: false);
        }
    }
}
