using System;

namespace OSA
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
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
