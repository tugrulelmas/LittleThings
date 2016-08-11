using AbiokaLittleThingsApi.Cache;
using AbiokaLittleThingsApi.Entity;
using System.Collections.Generic;

namespace AbiokaLittleThingsApi.Controllers
{
    public class EksiSozlukController : BaseApiController
    {
        public IEnumerable<Entry> Get() {
            var entries = new List<Entry>();
            //load first entry
            EksiSozlukCache.GetEntry(1);
            return EksiSozlukCache.Entries;
        }

        public Entry Get(int id) {
            return EksiSozlukCache.GetEntry(id);
        }
    }
}
