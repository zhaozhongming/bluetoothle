using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.BluetoothLE;
using Prism.Navigation;
using ReactiveUI;
using Samples.Infrastructure;

namespace Samples
{
    public class APHomeViewModel : ViewModel
    {
        readonly IAdapterScanner adapterScanner;
        readonly INavigationService navigationService;

        public APHomeViewModel(INavigationService navigationService,
                                    IAdapterScanner adapterScanner)
        {
            this.adapterScanner = adapterScanner;
            this.navigationService = navigationService;

            this.SelectS8 = ReactiveCommand.CreateFromTask(navigationService.NavToAdapterAPWorkS8);

            this.Select3000 = ReactiveCommand.CreateFromTask(navigationService.NavToAdapterAPWork3000);
        }

        public ICommand SelectS8 { get; }

        public ICommand Select3000 { get; }
    }
}
