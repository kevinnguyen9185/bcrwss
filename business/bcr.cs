using business.DAL;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace business
{
    public class bcr
    {
        public void UpsertTableMetadata(Input.Lobby.Table tableInfo, string vendor = "EVO")
        {
            if (tableInfo == null)
            {
                return;
            }

            using (var db = new LiteDatabase(Config.DataPath))
            {
                var tableId = tableInfo.tableId;
                var tableName = tableInfo.name;
                var bcrTableCollection = db.GetCollection<Entity.BcrTable>();
                var tableInDb = bcrTableCollection.FindOne(r => r.TableId == tableId);

                if (tableInDb == null)
                {
                    var newTable = new Entity.BcrTable
                    {
                        TableId = tableId,
                        Sessions = new List<Entity.RoadSession>(),
                        TableName = tableName,
                        Vendor = vendor,
                    };
                    bcrTableCollection.Insert(newTable);
                }
                else
                {
                    tableInDb.TableName = tableName;
                    tableInDb.Vendor = vendor;

                    bcrTableCollection.Update(tableInDb);
                }
            }
        }

        /// <summary>
        /// EVO specific
        /// </summary>
        /// <param name="road"></param>
        public void UpsertTableRoad(Input.BcrRoad.Road road)
        {
            if(road==null || string.IsNullOrEmpty(road.args.tableId))
            {
                return;
            }

            var tableId = road.args.tableId;
            var resultString = ToResultString(road);

            using(var db = new LiteDatabase(Config.DataPath))
            {
                var bcrTableCollection = db.GetCollection<Entity.BcrTable>();
                var tableInDb = bcrTableCollection.FindOne(r => r.TableId == tableId);

                if(tableInDb == null)
                {
                    // Need to insert new item
                    var newTableInInDb = new Entity.BcrTable
                    {
                        TableId = tableId,
                        Sessions = new List<Entity.RoadSession>
                        {
                           CreateNewSession(resultString)
                        }
                    };
                    bcrTableCollection.Insert(newTableInInDb);
                    bcrTableCollection.EnsureIndex(r => r.TableId);
                    return;
                }

                // Need to update
                if(tableInDb.Sessions == null)
                {
                    tableInDb.Sessions = new List<Entity.RoadSession>();
                }
                // Check if latest session result is similar to input
                var latestSessionResultInDb = tableInDb.Sessions.Count > 0 ? tableInDb.Sessions[tableInDb.Sessions.Count - 1] : null;
                if (latestSessionResultInDb != null
                    && !string.IsNullOrEmpty(latestSessionResultInDb.ResultString)
                    && resultString.IndexOf(latestSessionResultInDb.ResultString) == 0)
                {
                    // If session result occour in the begining of the result string, update session
                    latestSessionResultInDb.ResultString = resultString;
                    bcrTableCollection.Update(tableInDb);
                    return;
                }
                // Add new session in case result is different
                if (resultString.Length > 0)
                {
                    var newSession = CreateNewSession(resultString);
                    tableInDb.Sessions.Add(newSession);
                    bcrTableCollection.Update(tableInDb);
                }
            }
        }

        /// <summary>
        /// MGC specific
        /// </summary>
        /// <param name="tableId">tableId</param>
        /// <param name="result">result of each table</param>
        public void UpsertTablesession(string tableId, string result, int roundCount)
        {
            using (var db = new LiteDatabase(Config.DataPath))
            {
                var bcrTableCollection = db.GetCollection<Entity.BcrTable>();
                var tableInDb = bcrTableCollection.FindOne(r => r.TableId.ToLower() == tableId.ToLower());

                if (tableInDb == null) return;

                // Need to update
                if (tableInDb.Sessions == null)
                {
                    tableInDb.Sessions = new List<Entity.RoadSession>();
                }

                if (roundCount == 1)
                {
                    // Create new session
                    var newSession = new Entity.RoadSession
                    {
                        ResultString = result,
                        Dts = DateTime.UtcNow,
                        SessionId = Guid.NewGuid().ToString()
                    };
                    tableInDb.Sessions.Add(newSession);
                    bcrTableCollection.Update(tableInDb);
                }
                else
                {
                    // Update to current session
                    var latestSessionResultInDb = tableInDb.Sessions.Count > 0 ? tableInDb.Sessions[tableInDb.Sessions.Count - 1] : null;
                    if (latestSessionResultInDb != null
                        && !string.IsNullOrEmpty(latestSessionResultInDb.ResultString)
                        && result.IndexOf(latestSessionResultInDb.ResultString) == 0)
                    {
                        latestSessionResultInDb.ResultString = result;
                        bcrTableCollection.Update(tableInDb);
                        return;
                    }

                    if (result.Length > 0)
                    {
                        var newSession = new Entity.RoadSession
                        {
                            ResultString = result,
                            Dts = DateTime.UtcNow,
                            SessionId = Guid.NewGuid().ToString()
                        };
                        tableInDb.Sessions.Add(newSession);
                        bcrTableCollection.Update(tableInDb);
                    }
                }
            }
        }

        public List<Entity.BcrTable> GetBcrTableInfo()
        {
            using (var db = new LiteDatabase(Config.DataPath))
            {
                return db.GetCollection<Entity.BcrTable>().FindAll().ToList();
            }
        }

        public void CleanUpData()
        {
            using (var db = new LiteDatabase(Config.DataPath))
            {
                var bcrTableCollection = db.GetCollection<Entity.BcrTable>();
                var tables  = bcrTableCollection.FindAll().ToList();
                foreach (var table in tables)
                {
                    table.Sessions.RemoveAll(r => r.ResultString == null || r.ResultString.Trim() == "");
                    bcrTableCollection.Update(table);
                }
            }
        }

        private Entity.RoadSession CreateNewSession(string resultString)
        {
            var newSession = new Entity.RoadSession
            {
                ResultString = resultString,
                Dts = DateTime.UtcNow,
                SessionId = Guid.NewGuid().ToString()
            };
            return newSession;
        }

        private string ToResultString(Input.BcrRoad.Road road)
        {
            string result = "";
            foreach (var r in road.args.bigRoad)
            {
                if (r.color != null && r.color.ToLower() == "blue")
                {
                    result += "P";
                }
                else if (r.color != null && r.color.ToLower() == "red")
                {
                    result += "B";
                }
            }
            return result;
        }
    }
}