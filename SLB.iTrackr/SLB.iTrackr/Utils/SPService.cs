using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SLB.iTrackr.Models;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using SLB.iTrackr.Configs;

namespace SLB.iTrackr.Utils
{
    public class SPService
    {
        private HttpClient _httpClient;
        private string _baseURL;
        private string _ticketsSuffix = " &$expand=Client,Attachments" +
                                        " &$select=IDistrictJobID,DistrictValue,SubSegmentValue,WellName,JobStartDate,JobEndDate,JobType,Client/Title," +
                                        "CurrencyValue,TicketValue,FTLNumber,Id," +
                                        "Task1DueDate,Task2DueDate,Task3DueDate,Task4DueDate,Task5DueDate,Task6DueDate," +
            //"Task1CompleteDate,Task2CompleteDate,Task3CompleteDate,Task4CompleteDate,Task5CompleteDate,Task6CompleteDate," +
                                        "Task1Done,Task2Done,Task3Done,Task4Done,Task5Done,Task6Done," +
                                        "JPShipmentDate,JPShipmentTypeValue,Remarks,TicketStatusValue,Attachments" +
                                        " &$orderby=Created asc";

        public SPService(Credential credential, string url)
        {
            _baseURL = url;

            _httpClient = new HttpClient(new HttpClientHandler
            {
                Credentials = new NetworkCredential(credential.UserName, credential.Password, "DIR")
            });

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");
        }

        public async Task<List<Client>> GetClients()
        {
            var result = new List<Client>();
            var requestURL = _baseURL + "/_vti_bin/listdata.svc/Clients()?$select=Id,Title";

            try
            {
                var response = await _httpClient.GetAsync(requestURL);

                if (response.IsSuccessStatusCode)
                {
                    var contentTemp = await response.Content.ReadAsStringAsync();
                    var content = JsonConvert.DeserializeObject<Models.ClientsContent.RootObject>(contentTemp);

                    foreach (var client in content.d.results)
                    {
                        var clientTemp = new Client();
                        clientTemp.Id = client.Id;
                        clientTemp.Title = client.Title;

                        result.Add(clientTemp);
                    }

                    return result;
                }

                else
                {
                    MessagingCenter.Send<SPService, string>(this, "SP_REQUEST_STATUS", response.StatusCode.ToString());
                }
            }

            catch (Exception error)
            {
                MessagingCenter.Send<SPService, string>(this, "SP_REQUEST_STATUS", error.Message);
            }

            return null;
        }

        public async Task<List<Ticket>> GetTicketByStatus(string status, List<string> clients)
        {
            string clientFilter = "";

            #region __CONSTRUCT QUERY__
            if (clients != null)
            {
                foreach (var c in clients)
                {
                    var index = clients.IndexOf(c);

                    if (index == clients.Count() - 1)
                        clientFilter = clientFilter + "ClientId eq " + c;
                    else
                        clientFilter = clientFilter + "ClientId eq " + c + " or ";
                }
            }

            var requestURL = _baseURL + "/_vti_bin/listdata.svc/Tickets()?$filter=(TicketStatusValue eq '" +
                             status + "') and (" + clientFilter + ")" + _ticketsSuffix;
            #endregion

            return await ExecuteTicketsQuery(requestURL);
        }

        public async Task<List<Ticket>> GetTicketByParam (string param)
        {
            var requestURL = _baseURL + "/_vti_bin/listdata.svc/Tickets()?$filter=" + 
                "(substringof('" + param + "',FTLNumber)) or (substringof('" + param + "',IDistrictJobID))" + _ticketsSuffix;

            return await ExecuteTicketsQuery(requestURL);
        }

        public async Task SaveTicket(int ticketID, JObject bodyToSend)
        {
            var requestURL = (_baseURL + "/_vti_bin/listdata.svc/Tickets(" + ticketID + ")");
            var content = new StringContent(bodyToSend.ToString(), System.Text.Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Add("X-HTTP-Method", "MERGE");
            _httpClient.DefaultRequestHeaders.Add("If-Match", "*");

            try
            {
                var response = await _httpClient.PostAsync(requestURL, content);

                if(response.IsSuccessStatusCode)
                    MessagingCenter.Send<SPService, string>(this, "SP_REQUEST_STATUS", "Modification Saved !!");
                else
                    MessagingCenter.Send<SPService, string>(this, "SP_REQUEST_STATUS", "Could not modify ticket !!");
            }
            catch (Exception error)
            {
                MessagingCenter.Send<SPService, string>(this, "SP_REQUEST_STATUS", error.Message);
            }
        }

        private Color GetColorBasedOnDueDate(DateTime? dateToCheck)
        {
            Color result = new Color();

            if (dateToCheck != null)
            {

                if (DateTime.Now < dateToCheck)
                {
                    result = Color.FromHex(ColorScheme.OnTarget);
                }
                else if (DateTime.Now >= dateToCheck)
                {
                    result = Color.FromHex(ColorScheme.OffTarget);
                }

                return result;
            }

            return result;
        }

        private async Task<List<Ticket>> ExecuteTicketsQuery(string query)
        {
            try
            {
                var response = await _httpClient.GetAsync(query);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var contentTemp = JsonConvert.DeserializeObject<SLB.iTrackr.Models.TicketContent.RootObject>(content);

                    if (contentTemp.d.results.Count > 0)
                        return PopulateTickets(contentTemp.d.results);

                    else
                        MessagingCenter.Send<SPService, string>(this, "SP_REQUEST_STATUS", "No Ticket!");
                }
                else
                {
                    MessagingCenter.Send<SPService, string>(this, "SP_REQUEST_STATUS", response.StatusCode.ToString());
                }
            }
            catch (Exception error)
            {
                MessagingCenter.Send<SPService, string>(this, "SP_REQUEST_STATUS", error.Message);
            }

            return null;
        }

        private List<Ticket> PopulateTickets(List<Models.TicketContent.Result> r)
        {
            var result = new List<Ticket>();

            foreach (var t in r)
            {
                Ticket ticketTemp = new Ticket();

                ticketTemp.Id = t.Id;
                
                ticketTemp.IDistrictJobID = t.IDistrictJobID;
                ticketTemp.District = t.DistrictValue;
                ticketTemp.SubSegment = t.SubSegmentValue;
                ticketTemp.WellName = t.WellName;
                ticketTemp.JobStartDate = t.JobStartDate;
                ticketTemp.JobEndDate = t.JobEndDate;
                ticketTemp.JobType = t.JobType;
                ticketTemp.Client = t.Client.Title;
                ticketTemp.Currency = t.CurrencyValue;
                ticketTemp.TicketValue = t.TicketValue;
                ticketTemp.FTLNumber = t.FTLNumber;
                ticketTemp.Task1DueDate = CheckDateIsNotEmpty(t.Task1DueDate);
                ticketTemp.Task2DueDate = CheckDateIsNotEmpty(t.Task2DueDate);
                ticketTemp.Task3DueDate = CheckDateIsNotEmpty(t.Task3DueDate);
                ticketTemp.Task4DueDate = CheckDateIsNotEmpty(t.Task4DueDate);
                ticketTemp.Task5DueDate = CheckDateIsNotEmpty(t.Task5DueDate);
                ticketTemp.Task6DueDate = CheckDateIsNotEmpty(t.Task6DueDate);
                ticketTemp.Task1Done = t.Task1Done;
                ticketTemp.Task2Done = t.Task2Done;
                ticketTemp.Task3Done = t.Task3Done;
                ticketTemp.Task4Done = t.Task4Done;
                ticketTemp.Task5Done = t.Task5Done;
                ticketTemp.Task6Done = t.Task6Done;
                ticketTemp.JPShipmentType = t.JPShipmentTypeValue;

                if (t.JPShipmentDate != null)
                    ticketTemp.JPShipmentDate = t.JPShipmentDate;
                else
                    ticketTemp.JPShipmentDate = DateTime.Today;

                ticketTemp.TicketStatus = t.TicketStatusValue;
                ticketTemp.Remarks = t.Remarks;
                ticketTemp.ListViewDetail = t.IDistrictJobID + " | " + t.FTLNumber + " | " + t.CurrencyValue + " " + t.TicketValue;

                switch (t.TicketStatusValue)
                {
                    case "Job Package Completion":
                        ticketTemp.StatusColor = GetColorBasedOnDueDate(CheckDateIsNotEmpty(t.Task1DueDate));
                        ticketTemp.DueDate = CheckDateIsNotEmpty(t.Task1DueDate);
                        break;
                    case "Ticket Shipment to Town":
                        ticketTemp.StatusColor = GetColorBasedOnDueDate(CheckDateIsNotEmpty(t.Task2DueDate));
                        ticketTemp.DueDate = CheckDateIsNotEmpty(t.Task2DueDate);
                        break;
                    case "Compile Job Package":
                        ticketTemp.StatusColor = GetColorBasedOnDueDate(CheckDateIsNotEmpty(t.Task3DueDate));
                        ticketTemp.DueDate = CheckDateIsNotEmpty(t.Task3DueDate);
                        break;
                    case "PI Creation":
                        ticketTemp.StatusColor = GetColorBasedOnDueDate(CheckDateIsNotEmpty(t.Task4DueDate));
                        ticketTemp.DueDate = CheckDateIsNotEmpty(t.Task4DueDate);
                        break;
                    case "Client Submission Review":
                        ticketTemp.StatusColor = GetColorBasedOnDueDate(CheckDateIsNotEmpty(t.Task5DueDate));
                        ticketTemp.DueDate = CheckDateIsNotEmpty(t.Task5DueDate);
                        break;
                    case "Invoice Creation":
                        ticketTemp.StatusColor = GetColorBasedOnDueDate(CheckDateIsNotEmpty(t.Task6DueDate));
                        ticketTemp.DueDate = CheckDateIsNotEmpty(t.Task6DueDate);
                        break;
                }
                result.Add(ticketTemp);
            }

            return result;
        }

        private DateTime CheckDateIsNotEmpty(object date)
        {
            if (date != null)
                return (DateTime)date;
            else
                return DateTime.Now;
        }
    }
}
