using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Mauiapp1.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        private bool _isBusy;
        private string _title;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        // Helper method for async operations
        protected async Task ExecuteAsync(Task task, bool showBusy = true)
        {
            if (showBusy)
                IsBusy = true;

            try
            {
                await task;
            }
            finally
            {
                if (showBusy)
                    IsBusy = false;
            }
        }

        // Helper method for executing a function asynchronously
        protected async Task<TResult> ExecuteAsync<TResult>(Task<TResult> task, bool showBusy = true)
        {
            if (showBusy)
                IsBusy = true;

            try
            {
                return await task;
            }
            finally
            {
                if (showBusy)
                    IsBusy = false;
            }
        }
    }
}