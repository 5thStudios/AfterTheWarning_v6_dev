using System.Collections.Generic;

namespace Models
{
    public class MsgList
    {
        public int? VisionaryId { get; set; }
        public string VisionaryName { get; set; }

        public List<MsgLink> lstMsgLinks { get; set; } = new List<MsgLink>();
        public Pagination Pagination { get; set; } = new Pagination();
    }
}