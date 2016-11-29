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
	public class DateTimeTests
	{
		[Test]
		public void DateTimeIsAddedByDefault()
		{
			var provider = Substitute.For<IPropertiesProvider>();
			var options = Substitute.For<IMixpanelOptions>();
			options.Enabled.Returns(true);

			var eventTracker = Substitute.For<IEventTracker>();
			var timeProvider = Substitute.For<ITimeProvider>();
			timeProvider.UtcTime.Returns(new DateTime(2010, 12, 31, 13, 59, 30, DateTimeKind.Utc));

			var service = new MixpanelService(provider, options, timeProvider, eventTracker);

			service.SendEvent("testEvent");

			eventTracker.Received().Track(
				Arg.Is("testEvent"),
				Arg.Is<IDictionary<string, object>>(
					p => p.ContainsKey("DateTime") && string.Equals((string)p["DateTime"], "2010-12-31T13:59:30")
					));
		}

		[Test]
		public void DateTimeIsNotOverridenIfAlreadySupplied()
		{
			var provider = Substitute.For<IPropertiesProvider>();
			var options = Substitute.For<IMixpanelOptions>();
			options.Enabled.Returns(true);

			var eventTracker = Substitute.For<IEventTracker>();
			var timeProvider = Substitute.For<ITimeProvider>();
			timeProvider.UtcTime.Returns(DateTime.UtcNow);

			var service = new MixpanelService(provider, options, timeProvider, eventTracker);

			service.SendEvent("testEvent", new Dictionary<string, object> {
				{ "DateTime", new DateTime(2010, 12, 31, 13, 59, 30, DateTimeKind.Utc) }
			});

			eventTracker.Received().Track(
				Arg.Is("testEvent"),
				Arg.Is<IDictionary<string, object>>(
					p => p.ContainsKey("DateTime") && DateTime.Equals((DateTime)p["DateTime"], new DateTime(2010, 12, 31, 13, 59, 30, DateTimeKind.Utc))
					));
		}
	}
}
