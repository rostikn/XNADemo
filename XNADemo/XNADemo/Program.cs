using System;
using System.Reflection;

namespace Cybertone.XNA40Demo
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            const string windowTitleTemplate = "{0} v{1}";
            string windowTitle = string.Format(windowTitleTemplate, 
                Assembly.GetExecutingAssembly().GetName().Name, 
                Assembly.GetExecutingAssembly().GetName().Version);

            using (MainScene game = new MainScene())
            {
                game.Window.Title = windowTitle;
                game.Run();
            }
        }
    }
#endif
}

