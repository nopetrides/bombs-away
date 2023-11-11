using Murder;
using Murder.Diagnostics;

namespace BombsAway
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                using Game game = new(new BombsAwayGame());
                game.Run();
            }
            catch (Exception ex) when (GameLogger.CaptureCrash(ex)) { }
        }
    }
}
