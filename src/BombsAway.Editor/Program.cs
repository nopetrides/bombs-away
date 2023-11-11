using Murder.Editor;

namespace BombsAway.Editor
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var editor = new Architect(new BombsAwayArchitect()))
            {
                editor.Run();
            }
        }
    }
}
