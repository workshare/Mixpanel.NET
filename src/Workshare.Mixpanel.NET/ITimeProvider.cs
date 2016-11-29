using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshare.Mixpanel.NET
{
	public interface ITimeProvider
	{
		DateTime UtcTime { get; }
	}
}
