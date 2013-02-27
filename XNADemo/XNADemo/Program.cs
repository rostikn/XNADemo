using System;

namespace XNADemo
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MainScene game = new MainScene())
            {
                game.Run();
            }
        }
    }
#endif
}

