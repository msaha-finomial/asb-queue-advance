using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public class QueueMessage
    {
        public int Value { get; set; }
        public DateTime QueuedTime { get; set; }
        public string ByPerson { get; set; }
    }
}
