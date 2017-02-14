using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;

namespace GenesisEngine
{
    public static class Bootstrapper
    {
        public static Container Container { get; private set; }

        public static void BootstrapStructureMap()
        {
            Container = new Container(new GenesisRegistry());
        }
    }
}
