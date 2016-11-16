using Mixpanel.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Workshare.Mixpanel.NET
{
	internal class RequestStrategyFactory
	{
		public int? Timeout { get; set; }
		public string UserAgent { get; set; }
		public ICredentials Credentials { get; set; }
		public IWebProxy Proxy { get; set; }

		public IEnumerable<IHttpWebRequestStrategy> GetStrategies()
		{
			var decorators = new List<IHttpWebRequestStrategy>();

			if (!string.IsNullOrWhiteSpace(UserAgent))
			{
				decorators.Add(new UserAgentStrategy(UserAgent));
			}

			if (Proxy != null)
			{
				decorators.Add(new ProxyStrategy(Proxy, Credentials));
			}

			if (Timeout != null)
			{
				decorators.Add(new TimeoutStrategy(Timeout.Value));
			}

			return decorators;
		}
	}
}
