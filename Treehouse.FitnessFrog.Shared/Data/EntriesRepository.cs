using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Treehouse.FitnessFrog.Shared.Models;

namespace Treehouse.FitnessFrog.Shared.Data
{
    /// <summary>
    /// The repository for entries.
    /// </summary>
    public class EntriesRepository : BaseRepository<Entry>
    {
        public EntriesRepository(Context context) 
            : base(context)
        {
        }

        /// <summary>
        /// Returns a single entry for the provided ID.
        /// </summary>
        /// <param name="id">The ID for the entry to return.</param>
        /// <param name="userId">The user ID for the entry to return.</param>
        /// <param name="includeRelatedEntities">Indicates whether or not to include related entities.</param>
        /// <returns>An entry.</returns>
        public Entry Get(int id, string userId, bool includeRelatedEntities = true)
        {
            var entries = Context.Entries.AsQueryable();

            if (includeRelatedEntities)
            {
                entries = entries
                    .Include(e => e.Activity);
            }

            return entries
                .Where(e => e.Id == id && e.UserId == userId)
                .SingleOrDefault();
        }

        /// <summary>
        /// Returns a collection of entries.
        /// </summary>
        /// <returns>A list of entries.</returns>
        public IList<Entry> GetList(string userId)
        {
            return Context.Entries
                .Include(e => e.Activity)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ThenByDescending(e => e.Id)
                .ToList();
        }

        /// <summary>
        /// Determines if an entry is owned by the provided user ID.
        /// </summary>
        /// <param name="id">The entry ID to check.</param>
        /// <param name="userId">The user ID that should own the entry.</param>
        /// <returns>Returns a boolean indicating if the user ID owns the entry.</returns>
        public bool EntryOwnedByUserId(int id, string userId)
        {
            return Context.Entries
                .Where(e => e.Id == id && e.UserId == userId)
                .Count() == 1;
        }
    }
}