using Mixpanel.NET.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshare.Mixpanel.NET
{
	internal class NullEventTracker : IEventTracker
	{
		public bool Track(MixpanelEvent @event)
		{
			return true;
		}

		public bool Track(string @event, IDictionary<string, object> properties)
		{
			return true;
		}

		public bool Track<T>(T @event)
		{
			return true;
		}
	}
}
