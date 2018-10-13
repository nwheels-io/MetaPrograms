using System.Threading.Tasks;

namespace Example.App.Services
{
    public interface IGreetingService
    {
        Task<string> GetGreetingForName(string name);
    }
}
