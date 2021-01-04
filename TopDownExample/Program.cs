using System;

namespace TopDownExample
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new TopDownGame())
                game.Run();
        }
    }
}
