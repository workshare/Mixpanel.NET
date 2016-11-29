using Mixpanel.NET;
using Mixpanel.NET.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Workshare.Mixpanel.NET.Model;

namespace Workshare.Mixpanel.NET
{
	public class MixpanelService
	{
		private IPropertiesProvider _defaultPropertiesProvider;
		private IEventTracker _tracker;
		private IMixpanelOptions _options;
		private ITimeProvider _timeProvider;

		public MixpanelService(IMixpanelOptions options)
			: this(options, new NullPropertiesProvider())
		{
		}

		public MixpanelService(IMixpanelOptions options, IPropertiesProvider defaultPropertiesProvider)
			: this(options, defaultPropertiesProvider, new Clock(), GetTracker(options))
		{
		}

		internal MixpanelService(IMixpanelOptions options, IPropertiesProvider defaultPropertiesProvider, ITimeProvider timeProvider)
			: this(options, defaultPropertiesProvider, timeProvider, GetTracker(options))
		{
		}

		internal MixpanelService(IMixpanelOptions options, IPropertiesProvider defaultPropertiesProvider, ITimeProvider timeProvider, IEventTracker eventTracker)
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

		public void SendEvent(string eventName, string property, object value)
		{
			SendEvent(eventName, new Dictionary<string, object> { { property, value } });
		}

		public Task SendEventAsync(string eventName, string property, object value)
		{
			return SendEventAsync(eventName, new Dictionary<string, object> { { property, value } });
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

				if (!string.IsNullOrWhiteSpace(_options.UserAgent) && !eventProperties.Keys.Contains(EventProperties.UserAgent))
				{
					eventProperties[EventProperties.UserAgent] = _options.UserAgent;
				}

				if (_timeProvider != null && !eventProperties.Keys.Contains(EventProperties.DateTime))
				{
					eventProperties[EventProperties.DateTime] = _timeProvider.UtcTime.ToString("yyyy-MM-ddTHH:mm:ss");
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