using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;


namespace SLB.iTrackr.Models
{
    [ImplementPropertyChanged]
    public class TaskPageParam
    {
        public string Title { get; set; }
        public Credential Credential { get; set; }
        public string URL { get; set; }
        public List<string> ClientsId { get; set; }
    }
}
