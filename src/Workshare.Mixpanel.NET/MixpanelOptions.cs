﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Workshare.Mixpanel.NET
{
	public interface IMixpanelOptions
	{
		bool Enabled { get; set; }
		int? Timeout { get; set; }
		string Url { get; }
		string Token { get; }
		string UserAgent { get; }
		ICredentials Credentials { get; }
		IWebProxy Proxy { get; }
	}

	public class MixpanelOptions : IMixpanelOptions
	{
		public bool Enabled { get; set; } = true;
		public int? Timeout { get; set; }
		public string Url { get; set; }
		public string Token { get; set; }
		public string UserAgent { get; set; }
		public ICredentials Credentials { get; set; } = CredentialCache.DefaultNetworkCredentials;
		public IWebProxy Proxy { get; set; } = WebRequest.GetSystemWebProxy();
	}
}
