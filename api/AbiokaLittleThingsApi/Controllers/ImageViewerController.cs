using System;
using System.Collections.Generic;
using System.Linq;
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
            validHosts = new List<string> { "imgur.com",
            "hizliresim.com",
            "prntscr.com",
            "twimg.com",
            "postimg.org",
            "tinypic.com" };
        }

        /// <summary>
        /// Gets the image from specified URL.
        /// </summary>
        /// <remarks>
        /// Image from specified url.
        /// </remarks>
        /// <param name="url">The URL. It is must be in valid hosts. </param>
        /// <returns></returns>
        /// <response code="200">image from specified url.</response>
        /// <response code="400">Bad request. Host name of the specified url is not valid.</response>
        /// <response code="500">Internal Server Error</response>
        [Route("")]
        public async Task<HttpResponseMessage> Get([FromUri] string url) {
            var uri = new Uri(url);
            if (!validHosts.Contains(uri.Host) && !validHosts.Contains(string.Join(".", uri.Host.Split('.').Skip(1)))) {
                return Request.CreateResponse(HttpStatusCode.BadRequest, $"{uri.Host} is not supported");
            }

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