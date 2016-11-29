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
		private IMixpanelOptions _options;
		private ITimeProvider _timeProvider;

		public MixpanelService(IPropertiesProvider defaultPropertiesProvider, IMixpanelOptions options)
			: this(defaultPropertiesProvider, options, new Clock(), GetTracker(options))
		{
		}

		public MixpanelService(IPropertiesProvider defaultPropertiesProvider, IMixpanelOptions options, ITimeProvider timeProvider)
			: this(defaultPropertiesProvider, options, timeProvider, GetTracker(options))
		{
		}

		public MixpanelService(IPropertiesProvider defaultPropertiesProvider, IMixpanelOptions options, ITimeProvider timeProvider, IEventTracker eventTracker)
		{
			_defaultPropertiesProvider = defaultPropertiesProvider;
			_tracker = eventTracker;
			_options = options;
			_timeProvider = timeProvider;
		}

		private static IEventTracker GetTracker(IMixpanelOptions options)
		{
			if (!options.Enabled)
			{
				return new NullEventTracker();
			}

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

		public Task SendEventAsync(string eventName)
		{
			return SendEventAsync(eventName, new Dictionary<string, object>());
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

				if (!string.IsNullOrWhiteSpace(_options.UserAgent) && !eventProperties.Keys.Contains("User Agent"))
				{
					eventProperties["User Agent"] = _options.UserAgent;
				}

				if (_timeProvider != null && !eventProperties.Keys.Contains("DateTime"))
				{
					eventProperties["DateTime"] = _timeProvider.UtcTime.ToString("yyyy-MM-ddTHH:mm:ss");
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