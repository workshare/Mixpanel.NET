using Mixpanel.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Workshare.Mixpanel.NET
{
	internal class ProxyStrategy : IHttpWebRequestStrategy
	{
		private ICredentials _credentials;
		private IWebProxy _proxy;

		public ProxyStrategy(IWebProxy proxy, ICredentials credentials)
		{
			_proxy = proxy;
			_credentials = credentials;
		}

		public void Decorate(HttpWebRequest request)
		{
			request.Proxy = _proxy;
			request.Proxy.Credentials = _credentials;
		}
	}
}
