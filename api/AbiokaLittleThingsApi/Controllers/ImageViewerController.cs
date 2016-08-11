using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AbiokaLittleThingsApi.Controllers
{
    [RoutePrefix("api/ImageViewer")]
    public class ImageWiewerController : BaseApiController
    {
        private readonly static List<string> validHosts;

        static ImageWiewerController() {
            validHosts = new List<string> { "imgur.com", "www.imgur.com", "i.imgur.com",
            "hizliresim.com", "www.hizliresim.com", "i.hizliresim.com"};
        }

        [Route("")]
        public async Task<HttpResponseMessage> Get([FromUri] string url) {
            var uri = new Uri(url);
            if (!validHosts.Contains(uri.Host))
                throw new NotSupportedException($"{uri.Host} is not supported");

            HttpClient client;
#if DEBUG
            var httpClientHandler = new HttpClientHandler
            {
                Proxy = new WebProxy("http://10.0.7.224:8080", true),
                UseProxy = true
            };
            client = new HttpClient(httpClientHandler);
#endif
#if !DEBUG
            client = new HttpClient();
#endif
            var result = await client.GetAsync(url);


            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StreamContent(await result.Content.ReadAsStreamAsync());
            response.Content.Headers.ContentType = result.Content.Headers.ContentType;
            return response;
        }
    }
}