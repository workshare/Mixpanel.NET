using Mixpanel.NET;
using Mixpanel.NET.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshare.Mixpanel.NET
{
	internal class WsMixpanelTracker : MixpanelTracker
	{
		public WsMixpanelTracker(string token, IMixpanelHttp http = null, TrackerOptions options = null)
			: base(token, http, options)
		{
		}

		protected override void StandardiseMixpanelTime(Dictionary<string, object> propertyBag)
		{
		}
	}
}
