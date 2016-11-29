using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshare.Mixpanel.NET
{
	internal interface ITimeProvider
	{
		DateTime UtcTime { get; }
	}
}
