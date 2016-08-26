using System;

namespace AbiokaLittleThingsApi.Entity
{
    /// <summary>
    /// The entry.
    /// </summary>
    public class Entry
    {
        /// <summary>
        /// Gets or sets the sorting.
        /// </summary>
        /// <value>
        /// The sorting.
        /// </value>
        public int Sorting { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>
        /// The author.
        /// </value>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the entry date.
        /// </summary>
        /// <value>
        /// The entry date.
        /// </value>
        public string EntryDate { get; set; }

        /// <summary>
        /// Gets or sets the entry number.
        /// </summary>
        /// <value>
        /// The entry number.
        /// </value>
        public string EntryNumber { get; set; }

        /// <summary>
        /// Gets or sets the loaded date.
        /// </summary>
        /// <value>
        /// The loaded date.
        /// </value>
        public DateTime LoadedDate { get; set; }

        /// <summary>
        /// Gets or sets the favorite count.
        /// </summary>
        /// <value>
        /// The favorite count.
        /// </value>
        public int FavoriteCount { get; set; }
    }
}