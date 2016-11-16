using Mixpanel.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Workshare.Mixpanel.NET
{
	internal class TimeoutStrategy : IHttpWebRequestStrategy
	{
		private int _timeoutMilliseconds;

		public TimeoutStrategy(int timeoutMilliseconds)
		{
			_timeoutMilliseconds = timeoutMilliseconds;
		}

		public void Decorate(HttpWebRequest request)
		{
			request.Timeout = _timeoutMilliseconds;
		}
	}
}
