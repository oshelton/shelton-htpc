using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheltonHTPC.FrontEndData
{
	/// <summary>
	/// An exception to be thrown when data that the Shelton HTPC front end expects to be found is missing.
	/// </summary>
	public class MissingRequiredDataException : InvalidOperationException
	{
		public MissingRequiredDataException(string message, bool fatal = true)
			: base(message)
		{
			fatal = Fatal;
		}

		/// <summary>
		/// Whether or not the missing data should result in the application terminating.
		/// </summary>
		public bool Fatal { get; private set; }
	}
}
