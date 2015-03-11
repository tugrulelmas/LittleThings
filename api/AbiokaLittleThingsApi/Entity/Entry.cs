using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AbiokaLittleThingsApi.Entity
{
    public class Entry
    {
        public int Sorting { get; set; }

        public string Url { get; set; }

        public string Author { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public string EntryDate { get; set; }

        public string EntryNumber { get; set; }

        public DateTime LoadedDate { get; set; }

        public int FavoriteCount { get; set; }
    }
}