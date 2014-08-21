using AbiokaLittleThingsApi.Cache;
using AbiokaLittleThingsApi.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AbiokaLittleThingsApi.Controllers
{
    public class EksiSozlukController : BaseApiController
    {
        public IEnumerable<Entry> Get() {
            var entries = new List<Entry>();
            foreach (var entryItem in EksiSozlukCache.Entries) {
                var entry = new Entry() {
                    Title = entryItem.Title,
                    Sorting = entryItem.Sorting,
                    Author = entryItem.Author,
                    Url = entryItem.Url,
                    LoadedDate = entryItem.LoadedDate
                };
                entries.Add(entry);
            }
            return entries;
        }

        public Entry Get(int id) {
            return EksiSozlukCache.GetEntry(id);
        }
    }
}
