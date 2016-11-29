## Workshare Mixpanel API Integration for .NET

This project is a Workshare domain specific wrapper around https://github.com/chrisnicola/Mixpanel.NET.

### Usage
```csharp

// initalise mixpanel settings
var options = new MixpanelOptions
{
	Url = "url to mixpanel proxy, if needed",
	Token = "mixpanel token", // not your api key,
	UserAgent = "{client}/{version number}"
};

// create a mixpanel service
var service = new MixpanelService(options);

// create a mixpanel service, supplying default properties for every event
IPropertiesProvider defaultPropertiesProvider = new YourPropertiesProvider();
var service = new MixpanelService(options, defaultPropertiesProvider);

// send an event
service.SendEvent("your event");

// send an event with a single property
service.SendEvent("your event", Model.EventProperties.Email, "user@asdf.com");

// send an event asynchronously with multiple properties
await service.SendEventAsync("your event", new Dictionary<string, object>
{
	{Model.EventProperties.Email, "user@asdf.com" },
	{Model.EventProperties.ActionName, "submit" }
});

```
### Service Behavour

This library is intended to encourage, but not enforce best practice (I don't have enough data to do this). As such:

* options.UserAgent specifies the user agent that is included in the request header and in the "UserAgent" event property.
* the "DateTime" property is generated when the event is generated. DateTime is in UTC (your Mixpanel project should be set to UTC accordingly).
* the system proxy and network credentials are used by default.

If a property is provided external to the library (either as a default property or with the event itself), then the provided values are used.

### Best Practice

The EventProperties class provides the list of properties currently used by the Workshare Analytics team. Only these properties are included in data warehousing. If your property is not present, talk to someone on that team to determine if an existing property can be overloaded, or if a new property can be added. See [Mixpanel Events.docx](docs/Mixpanel Events.docx)
