using BombsAway.Assets;
using Murder;
using Murder.Core;
using Murder.Diagnostics;
namespace BombsAway.Services
{
    internal static class SaveServices
    {
        public static BombsAwaySaveData GetOrCreateSave()
        {
#if DEBUG
            if (Game.Instance.ActiveScene is not GameScene && Game.Data.TryGetActiveSaveData() is null)
            {
                GameLogger.Warning("Creating a save out of the game!");
            }
#endif

            if (Game.Data.TryGetActiveSaveData() is not BombsAwaySaveData save)
            {
                // Right now, we are creating a new save if one is already not here.
                save = (BombsAwaySaveData)Game.Data.CreateSave("_default");
            }

            return save;
        }

        public static BombsAwaySaveData? TryGetSave() => Game.Data.TryGetActiveSaveData() as BombsAwaySaveData;

        public static void QuickSave() => Game.Data.QuickSave();
    }
}
