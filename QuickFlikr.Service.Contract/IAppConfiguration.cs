using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickFlikr.Service.Contract
{
    public interface IAppConfiguration
    {
        Uri FlikrServiceUrl { get; }
    }
}
