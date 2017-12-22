using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLB.iTrackr.Models.AttachmentContent
{

    public class Metadata
    {
        public string uri { get; set; }
        public string type { get; set; }
        public string edit_media { get; set; }
        public string media_src { get; set; }
        public string content_type { get; set; }
        public string media_etag { get; set; }
    }

    public class Result
    {
        public Metadata __metadata { get; set; }
        public string EntitySet { get; set; }
        public int ItemId { get; set; }
        public string Name { get; set; }
    }

}
