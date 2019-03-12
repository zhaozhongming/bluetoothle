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
                        { "adapter", CrossBleAdapter.Current }, { "knownId", x.Id }, { "modelType", selectedModel}, { "PreDefinedGuid", PreDefinedGuid}
                    })
           );
        }
        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            this.adapter = parameters.GetValue<IAdapter>("adapter");
            this.selectedModel = parameters.GetValue<ModelType>("modelType");

            PreDefinedGuid = new List<DeviceItem>();
            PreDefinedGuid.Add(new DeviceItem(new Guid("00000000-0000-0000-0000-ee5d6da44ba7"), "00000000-0000-0000-0000-ee5d6da44ba7(test1)"));
            PreDefinedGuid.Add(new DeviceItem(new Guid("00000000-0000-0000-0000-cc81d47f7569"), "00000000-0000-0000-0000-cc81d47f7569(test2)"));
            PreDefinedGuid.Add(new DeviceItem(new Guid("00000000-0000-0000-0000-d5e368beeb52"), "00000000-0000-0000-0000-d5e368beeb52(boxtest1)"));
            PreDefinedGuid.Add(new DeviceItem(new Guid("00000000-0000-0000-0000-d9bcfc833039"), "00000000-0000-0000-0000-d9bcfc833039(boxtest2)"));
            PreDefinedGuid.Add(new DeviceItem(new Guid("00000000-0000-0000-0000-d3625e655b9c"), "00000000-0000-0000-0000-d3625e655b9c(boxtest3)"));
            PreDefinedGuid.Add(new DeviceItem(new Guid("00000000-0000-0000-0000-cb6b72c108ab"), "00000000-0000-0000-0000-cb6b72c108ab(boxtest4)"));
            PreDefinedGuid.Add(new DeviceItem(new Guid("00000000-0000-0000-0000-d56b95b03038"), "00000000-0000-0000-0000-d56b95b03038(boxtest4)"));
            PreDefinedGuid.Add(new DeviceItem(new Guid("00000000-0000-0000-0000-000000000000"), "自 动 连 接"));
        }
    }
}
