using System.Collections.Generic;

namespace business.Entity
{
    public class BcrRoadSession
    {
        public string TableId { get; set; }
        public List<RoadSession> Sessions { get; set; }
    }

    public class RoadSession
    {
        public string SessionId { get; set; }
        public string ResultString { get; set; }
    }
}
