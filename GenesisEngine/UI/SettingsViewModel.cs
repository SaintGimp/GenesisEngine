using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public class SettingsViewModel
    {
        ISettings _settings;

        public SettingsViewModel(ISettings settings)
        {
            _settings = settings;
        }
    }
}
