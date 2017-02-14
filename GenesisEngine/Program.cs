using System;

namespace GenesisEngine
{
#if WINDOWS || LINUX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Bootstrapper.BootstrapStructureMap();

            using (var game = Bootstrapper.Container.GetInstance<Genesis>())
            {
                game.Run();
            }
        }
    }
#endif
}

