using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLB.iTrackr.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Selected { get; set; }

        public Client()
        {
            Selected = false;
        }
    }
}
