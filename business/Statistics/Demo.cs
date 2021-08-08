using business.DAL;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace business.Statistics
{
    public class Demo
    {
        public void CountPlayerBanker()
        {
            using (var db = new LiteDatabase(Config.DataPath))
            {
                var bcrTableCollection = db.GetCollection<Entity.BcrTable>();
                int noP = 0;
                int noB = 0;

                foreach (var tbl in bcrTableCollection.FindAll())
                {
                    //foreach (var ss in tbl.Sessions)
                    //{
                    //    noP += ss.ResultString.
                    //}
                }
            }
        }
    }


}
