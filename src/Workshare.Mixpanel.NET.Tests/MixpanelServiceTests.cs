using Mixpanel.NET.Events;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshare.Mixpanel.NET.Tests
{
	[TestFixture]
	public class MixpanelServiceTests
	{
		[Test]
		public void TrackEventSendsEventToTracker()
		{
			MixpanelOptions options = new MixpanelOptions 
			{
				Enabled = true
			};

			var tracker = Substitute.For<IEventTracker>();


			MixpanelService service = new MixpanelService(options, new NullPropertiesProvider(), null, tracker);
			service.SendEvent("eventName");

			tracker.Received().Track("eventName", Arg.Is<IDictionary<string, object>>(p => p.Count == 0));
		}

		[Test]
		public void TrackEventSendsEventPropertyToTracker()
		{
			MixpanelOptions options = new MixpanelOptions
			{
				Enabled = true
			};

			var tracker = Substitute.For<IEventTracker>();

			MixpanelService service = new MixpanelService(options, new NullPropertiesProvider(), null, tracker);
			service.SendEvent("eventName", "property", 123);

			tracker.Received().Track("eventName", Arg.Is<IDictionary<string, object>>(p => p.ContainsKey("property") && (int) p["property"] == 123));
		}

		[Test]
		public void TrackEventSendsEventPropertiesToTracker()
		{
			MixpanelOptions options = new MixpanelOptions
			{
				Enabled = true
			};

			var tracker = Substitute.For<IEventTracker>();

			MixpanelService service = new MixpanelService(options, new NullPropertiesProvider(), null, tracker);
			service.SendEvent("eventName", new Dictionary<string, object> { { "property", 123 } });

			tracker.Received().Track("eventName", Arg.Is<IDictionary<string, object>>(p => p.ContainsKey("property") && (int)p["property"] == 123));
		}
	}
}
