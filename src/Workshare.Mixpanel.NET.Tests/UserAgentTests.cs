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
	public class UserAgentTests
	{
		[Test]
		public void UserAgentIsAddedByDefault()
		{
			var provider = Substitute.For<IPropertiesProvider>();
			var options = Substitute.For<IMixpanelOptions>();
			options.Enabled.Returns(true);

			var userAgent = "testAgent";
			options.UserAgent.Returns(userAgent);
			var eventTracker = Substitute.For<IEventTracker>();
			var service = new MixpanelService(provider, options, eventTracker);

			service.SendEvent("testEvent");

			eventTracker.Received().Track(
				Arg.Is("testEvent"),
				Arg.Is<IDictionary<string, object>>(
					p => p.ContainsKey("User Agent") && string.Equals((string)p["User Agent"], userAgent)
					));
		}

		[Test]
		public void UserAgentIsNotOverridenIfAlreadySupplied()
		{
			var provider = Substitute.For<IPropertiesProvider>();
			var options = Substitute.For<IMixpanelOptions>();
			options.Enabled.Returns(true);

			var userAgent = "testAgent";
			options.UserAgent.Returns(userAgent);
			var eventTracker = Substitute.For<IEventTracker>();
			var service = new MixpanelService(provider, options, eventTracker);

			service.SendEvent("testEvent", new Dictionary<string, object> {
				{ "User Agent", "asdf" }
			});

			eventTracker.Received().Track(
				Arg.Is("testEvent"),
				Arg.Is<IDictionary<string, object>>(
					p => p.ContainsKey("User Agent") && string.Equals((string)p["User Agent"], "asdf")
					));
		}
	}
}
