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
                // At the moment MonoGame 3.6 doesn't support SharpDX 4, e.g. https://github.com/MonoGame/MonoGame/issues/3794
                // so if you crash here with an error that SharpDX types aren't found, that's probably the issue. I think this
                // will be addressed in MonoGame 3.7.
                game.Run();
            }
        }
    }
#endif
}

