using System;
using System.Collections.Generic;
using System.Text;

namespace business.Command
{
    public class TableInput
    {
        public string tableId { get; set; }
    }

    public class SubscribeTopic
    {
        public string topic { get; set; }
        public List<TableInput> tables { get; set; }
    }

    public class Args
    {
        public List<SubscribeTopic> subscribeTopics { get; set; }
        public List<object> unsubscribeTopics { get; set; }
    }

    public class SubscribeTableInfo
    {
        public string id { get; set; }
        public string type { get; set; }
        public Args args { get; set; }
    }
}
