using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshare.Mixpanel.NET
{
	public class NullPropertiesProvider : IPropertiesProvider
	{
		public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();
	}
}
