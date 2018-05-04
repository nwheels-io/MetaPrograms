using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Example.App
{
    public interface IGreetingService
    {
        Task<string> GetGreetingForName(string name);
    }
}
