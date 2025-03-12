using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mauiapp1.Services
{
    public interface INavigationService
    {
        Task NavigateToAsync(string route);
        Task NavigateBackAsync();
    }
}

