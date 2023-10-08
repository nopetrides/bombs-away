using Murder;
using HelloMurder;
using HelloMurder.Assets;

namespace HelloMurder.Services
{
    public static class LibraryServices
    {
        public static LibraryAsset GetLibrary()
        {
            return Game.Data.GetAsset<LibraryAsset>(HelloMurderGame.Profile.Library);
        }
    }
}
