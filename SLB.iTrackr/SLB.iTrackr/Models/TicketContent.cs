using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLB.iTrackr.Models.TicketContent
{

    public class Metadata
    {
        public string uri { get; set; }
        public string etag { get; set; }
        public string type { get; set; }
    }

    public class Metadata2
    {
        public string uri { get; set; }
        public string etag { get; set; }
        public string type { get; set; }
    }

    public class Client
    {
        public Metadata2 __metadata { get; set; }
        public string Title { get; set; }
    }

    public class Attachments
    {
        public List<AttachmentContent.Result> results { get; set; }
    }

    public class Result
    {
        public Metadata __metadata { get; set; }
        public int Id { get; set; }
        public string IDistrictJobID { get; set; }
        public string WellName { get; set; }
        public DateTime JobStartDate { get; set; }
        public DateTime JobEndDate { get; set; }
        public string JobType { get; set; }
        public Client Client { get; set; }
        public double TicketValue { get; set; }
        public string FTLNumber { get; set; }
        public bool Task2Done { get; set; }
        public bool Task3Done { get; set; }
        public DateTime? Task3CompleteDate { get; set; }
        public DateTime? Task2CompleteDate { get; set; }
        public bool Task1Done { get; set; }
        public DateTime? Task1CompleteDate { get; set; }
        public bool Task4Done { get; set; }
        public DateTime? Task4CompleteDate { get; set; }
        public bool Task5Done { get; set; }
        public DateTime? Task5CompleteDate { get; set; }
        public bool Task6Done { get; set; }
        public DateTime? Task6CompleteDate { get; set; }
        public string TicketStatusValue { get; set; }
        public DateTime? Task1DueDate { get; set; }
        public DateTime? Task2DueDate { get; set; }
        public DateTime? Task3DueDate { get; set; }
        public DateTime? Task4DueDate { get; set; }
        public DateTime? Task5DueDate { get; set; }
        public DateTime? Task6DueDate { get; set; }
        public string JPShipmentTypeValue { get; set; }
        public DateTime? JPShipmentDate { get; set; }
        public string Remarks { get; set; }
        public string SubSegmentValue { get; set; }
        public string DistrictValue { get; set; }
        public string CurrencyValue { get; set; }
        public Attachments Attachments { get; set; }
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
