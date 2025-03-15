using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mauiapp1.Services
{
    public interface INavigationService
    {
        Task NavigateToAsync(string route, IDictionary<string, object> parameters = null);
    }
}
