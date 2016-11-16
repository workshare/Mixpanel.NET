using Mixpanel.NET;
using Mixpanel.NET.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Workshare.Mixpanel.NET
{
	public class MixpanelService
	{
		private IPropertiesProvider _defaultPropertiesProvider;
		private IEventTracker _tracker;

		public MixpanelService(IPropertiesProvider defaultPropertiesProvider, IMixpanelOptions options)
			: this(defaultPropertiesProvider, GetTracker(options))
		{
		}

		public MixpanelService(IPropertiesProvider defaultPropertiesProvider, IEventTracker eventTracker)
		{
			_defaultPropertiesProvider = defaultPropertiesProvider;
			_tracker = eventTracker;
		}

		private static IEventTracker GetTracker(IMixpanelOptions options)
		{
			var decoratorFactory = new RequestStrategyFactory
			{
				Timeout = options.Timeout,
				Credentials = options.Credentials,
				Proxy = options.Proxy,
				UserAgent = options.UserAgent
			};
			return GetTracker(options, decoratorFactory.GetStrategies());
		}


		private static IEventTracker GetTracker(IMixpanelOptions options, IEnumerable<IHttpWebRequestStrategy> requestDecorators)
		{
			var mixpanelHttp = new MixpanelHttp(requestDecorators);
			return new WsMixpanelTracker(options.Token, mixpanelHttp, new TrackerOptions() { SetEventTime = true, ProxyUrl = options.Url });
		}

		public void SendEvent(string eventName)
		{
			SendEvent(eventName, new Dictionary<string, object>());
		}

		public void SendEvent(string eventName, IDictionary<string, object> properties)
		{

			try
			{
				var eventProperties = new Dictionary<string, object>(_defaultPropertiesProvider.Properties);

				foreach (var property in properties)
				{
					eventProperties[property.Key] = property.Value;
				}

				_tracker.Track(eventName, eventProperties);
			}
			catch (Exception ex)
			{
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Caught mixpanel exception");
				System.Diagnostics.Trace.WriteLine(ex.ToString());
#endif

			}
		}

		public Task SendEventAsync(string eventName, IDictionary<string, object> properties)
		{
			return Task.Run(() =>
			{
				var localEventName = eventName;
				var localProperties = properties;

				SendEvent(localEventName, localProperties);
			});
		}

	}
}