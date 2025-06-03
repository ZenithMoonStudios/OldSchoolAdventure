using System;
using System.Globalization;

namespace OSA
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
                using (PlatformerGame game = PlatformerGame.Instance)
                {
                    game.Run();
                }
            }
            catch (Exception e)
            {
                using (ExceptionGame game = new ExceptionGame(e))
                {
                    game.Run();
                }
            }
        }
    }
}
