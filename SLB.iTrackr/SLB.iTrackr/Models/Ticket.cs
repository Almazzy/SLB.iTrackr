using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SLB.iTrackr.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string IDistrictJobID { get; set; }
        public string District { get; set; }
        public string SubSegment { get; set; }
        public string WellName { get; set; }
        public DateTime JobStartDate { get; set; }
        public DateTime JobEndDate { get; set; }
        public string JobType { get; set; }
        public string Client { get; set; }
        public string Currency { get; set; }
        public double TicketValue { get; set; }
        public string FTLNumber { get; set; }
        public DateTime? Task1CompleteDate { get; set; }
        public DateTime? Task2CompleteDate { get; set; }
        public DateTime? Task3CompleteDate { get; set; }
        public DateTime? Task4CompleteDate { get; set; }
        public DateTime? Task5CompleteDate { get; set; }
        public DateTime? Task6CompleteDate { get; set; }
        public DateTime? Task1DueDate { get; set; }
        public DateTime? Task2DueDate { get; set; }
        public DateTime? Task3DueDate { get; set; }
        public DateTime? Task4DueDate { get; set; }
        public DateTime? Task5DueDate { get; set; }
        public DateTime? Task6DueDate { get; set; }
        public bool Task1Done { get; set; }
        public bool Task2Done { get; set; }
        public bool Task3Done { get; set; }
        public bool Task4Done { get; set; }
        public bool Task5Done { get; set; }
        public bool Task6Done { get; set; }
        public string JPShipmentType { get; set; }
        public DateTime? JPShipmentDate { get; set; }
        public String TicketStatus { get; set; }
        public string Remarks { get; set; }
        public Color StatusColor { get; set; }
        public DateTime? DueDate { get; set; }
        public string ListViewDetail { get; set; }
    }
}
