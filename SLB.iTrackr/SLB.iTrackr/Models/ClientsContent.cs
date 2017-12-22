using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLB.iTrackr.Models.ClientsContent
{
    public class Metadata
    {
        public string uri { get; set; }
        public string etag { get; set; }
        public string type { get; set; }
    }

    public class Result
    {
        public Metadata __metadata { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class D
    {
        public List<Result> results { get; set; }
    }

    public class RootObject
    {
        public D d { get; set; }
    }
}
