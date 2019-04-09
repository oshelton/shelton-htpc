using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SheltonHTPC.Data.Entities;
using WPFAspects.Core;

namespace SheltonHTPC
{
    public class MainWindowViewModel : Model
    {
        public async Task Initialize()
        {
            IsInitializing = true;
            var result = await GeneralSettings.Deserialize();

            GeneralSettings = result;
            IsInitializing = false;
        }

        private bool _IsInitializing = false;
        public bool IsInitializing
        {
            get => _IsInitializing;
            private set => SetPropertyBackingValue(value, ref _IsInitializing);
        }

        private GeneralSettings _GeneralSettings = null;
        public GeneralSettings GeneralSettings
        {
            get => _GeneralSettings;
            private set => SetPropertyBackingValue(value, ref _GeneralSettings);
        }
    }
}
