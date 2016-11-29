using Mixpanel.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Workshare.Mixpanel.NET
{
	internal class UserAgentStrategy : IHttpWebRequestStrategy
	{
		private string _userAgent;

		public UserAgentStrategy(string userAgent)
		{
			_userAgent = userAgent;
		}

		public void Decorate(HttpWebRequest request)
		{
			request.UserAgent = _userAgent;
		}
	}
}
