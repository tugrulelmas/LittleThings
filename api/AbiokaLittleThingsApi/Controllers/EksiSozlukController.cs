using AbiokaLittleThingsApi.Cache;
using AbiokaLittleThingsApi.Entity;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace AbiokaLittleThingsApi.Controllers
{
    public class EksiSozlukController : BaseApiController
    {
        /// <summary>
        /// Gets entities of DEBE list from eksisozluk.com
        /// </summary>
        /// <remarks>
        /// entities of DEBE list
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">An array of entities</response>
        /// <response code="500">Internal Server Error</response>
        [ResponseType(typeof(IEnumerable<Entry>))]
        public IEnumerable<Entry> Get() {
            var entries = new List<Entry>();
            //load first entry
            EksiSozlukCache.GetEntry(1);
            return EksiSozlukCache.Entries;
        }

        /// <summary>
        /// Gets the DEBE entity with id.
        /// </summary>
        /// <remarks>
        /// DEBE entity
        /// </remarks>
        /// <param name="id">entity id.</param>
        /// <returns></returns>
        /// <response code="200">entity</response>
        /// <response code="500">Internal Server Error</response>
        [ResponseType(typeof(Entry))]
        public Entry Get(int id) {
            return EksiSozlukCache.GetEntry(id);
        }
    }
}
