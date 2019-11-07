using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Mixpanel.NET
{
  /// <summary>
  /// This helper class and interface largly exists to improve readability and testability since there is 
  /// no way to do that with the WebRequest class cleanly.
  /// </summary>
  public interface IMixpanelHttp {
    string Get(string uri, string query);
    string Post(string uri, string body);
  }

  public interface IHttpWebRequestStrategy {
    void Decorate(HttpWebRequest request);
  }

  public class MixpanelHttp : IMixpanelHttp {
    private IEnumerable<IHttpWebRequestStrategy> _decorators;

    public MixpanelHttp()
      : this(null) { 
    }
    
    public MixpanelHttp(IEnumerable<IHttpWebRequestStrategy> decorators) { 
      if( decorators == null) {
        _decorators = new List<IHttpWebRequestStrategy>();
      } else { 
        _decorators = decorators;
      }
    }

    public string Get(string uri, string query) {

#if BUILD_FOR_PLATFORM_NET40
     //Do nothing
#elif BUILD_FOR_PLATFORM_NET452
     ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
#else
     ServicePointManager.SecurityProtocol |= SecurityProtocolType.SystemDefault;
#endif

      var request = WebRequest.Create(uri + "?" + query);
      foreach(var decorator in _decorators) {
        decorator.Decorate((HttpWebRequest)request);
      }
      var response = request.GetResponse();
      var responseStream = response.GetResponseStream();
      return responseStream == null 
        ? null
        : new StreamReader(responseStream).ReadToEnd();
    }

    public string Post(string uri, string body) {

#if BUILD_FOR_PLATFORM_NET40
     //Do nothing
#elif BUILD_FOR_PLATFORM_NET452
     ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
#else
     ServicePointManager.SecurityProtocol |= SecurityProtocolType.SystemDefault;
#endif

      var request = WebRequest.Create(uri);
      request.Method = "POST";
      request.ContentType = "application/x-www-form-urlencoded";
      foreach(var decorator in _decorators) {
        decorator.Decorate((HttpWebRequest)request);
      }
      var bodyBytes = Encoding.UTF8.GetBytes(body);
      request.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length);
      var response = request.GetResponse();
      var responseStream = response.GetResponseStream();
      return responseStream == null 
        ? null
        : new StreamReader(responseStream).ReadToEnd();
    }
  }
}