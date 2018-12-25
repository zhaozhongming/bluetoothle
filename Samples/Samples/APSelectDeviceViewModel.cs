using Plugin.BluetoothLE;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Samples
{
    public class DeviceItem
    {
        public DeviceItem(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class APSelectDeviceViewModel : ViewModel
    {
        IAdapter adapter;
        ModelType selectedModel;

        public ICommand SelectDevice { get; }

        [Reactive] public List<DeviceItem> PreDefinedGuid { get; set; }

        public APSelectDeviceViewModel(INavigationService navigationService)
        {
           SelectDevice = ReactiveCommand.CreateFromTask<DeviceItem>(
               x => navigationService.Navigate("APWorkPage", new NavigationParameters
                    {
                        { "adapter", CrossBleAdapter.Current }, { "knownId", x.Id }, { "modelType", selectedModel}
                    })
           );
        }
        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            this.adapter = parameters.GetValue<IAdapter>("adapter");
            this.selectedModel = parameters.GetValue<ModelType>("modelType");

            PreDefinedGuid = new List<DeviceItem>();
            PreDefinedGuid.Add(new DeviceItem(new Guid("00000000-0000-0000-0000-ee5d6da44ba7"), "00000000-0000-0000-0000-ee5d6da44ba7"));
            PreDefinedGuid.Add(new DeviceItem(new Guid("00000000-0000-0000-0000-cc81d47f7569"), "00000000-0000-0000-0000-cc81d47f7569"));
            PreDefinedGuid.Add(new DeviceItem(new Guid("00000000-0000-0000-0000-000000000000"), "我不知道"));
        }
    }
}
