using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshare.Mixpanel.NET
{
	internal class Clock : ITimeProvider
	{
		public DateTime UtcTime
		{
			get
			{
				return DateTime.UtcNow;
			}
		}
	}
}
