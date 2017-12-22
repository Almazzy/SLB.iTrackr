using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SLB.iTrackr.Models;

namespace SLB.iTrackr.Utils
{
    public interface ICredentialService
    {
        Boolean SaveCredential(Credential credential);
        Credential GetCredential();
    }
}
