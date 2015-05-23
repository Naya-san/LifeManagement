using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using LifeManagement.Models;

namespace LifeManagement.Logic
{
    public class GarbageCollectorDB
    {
         private static int frequency = 7;
        private readonly TimeSpan _updateInterval = TimeSpan.FromDays(7);
        private volatile ApplicationDbContext db = new ApplicationDbContext();

        public GarbageCollectorDB()
        {
            var t = System.Threading.Tasks.Task.Factory.StartNew(() => DeleteOldArchives());
        }

        private async System.Threading.Tasks.Task DeleteOldArchives()
        {
 	       while (true)
 	       {
 	            var today = DateTime.UtcNow.AddDays(-1*frequency);
 	            var listsOld = db.ListsForDays.Where(x => x.Date < today).Include(x => x.Archive).ToList();
 	           foreach (var old in listsOld)
 	           {
 	               db.Archives.RemoveRange(old.Archive);
                   old.Archive.Clear();
 	           }
                await db.SaveChangesAsync();
                Thread.Sleep(_updateInterval);
            }
        }
    }
}