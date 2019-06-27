using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheltonHTPC.Interfaces
{
    public interface IGeneralSettings
    {
		string DataPath { get; }
		uint IdleWaitMinutes { get; }
	}
}
