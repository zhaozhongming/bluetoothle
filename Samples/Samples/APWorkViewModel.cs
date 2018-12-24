using Plugin.BluetoothLE;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Reactive.Disposables;

namespace Samples
{
    public class APWorkViewModel : ViewModel
    {
        //private Guid knownId = new Guid("00000000-0000-0000-0000-cc81d47f7569 ");
        private Guid knownId = new Guid("00000000-0000-0000-0000-ee5d6da44ba7 ");
        private List<Guid> allowedId = new List<Guid>();

        const string serviceidHC = "0000ffe0-0000-1000-8000-00805f9b34fb";//taobeo HC-42
        static readonly Guid serviceguidHC = new Guid(serviceidHC);
        const string cidHC = "0000ffe1-0000-1000-8000-00805f9b34fb";//taobao bluetooth HC-42
        static readonly Guid wguid = new Guid(cidHC);

        //const string serviceidBLE = "0000ffe0-0000-1000-8000-00805f9b34fb";//ble
        //static readonly Guid serviceguidBLE = new Guid(serviceidBLE);
        //const string cidBLE = "00031234-0000-1000-8000-00805F9B0130";//ble
        //static readonly Guid cguidBLE = new Guid(cidBLE);
        //const string widBLE = "00031234-0000-1000-8000-00805F9B0131";//ble
        //static readonly Guid wguidBLE = new Guid(widBLE);

        #region 3000
        protected byte address = 0x00;

        public static readonly byte RXMODE = 0x70;
        public static readonly byte TXMODE = 0x68;

        public static readonly byte NUL = 0x00;
        public static readonly byte STX = 0x02;
        public static readonly byte ETX = 0x03;
        public static readonly byte EOT = 0x04;
        public static readonly byte ENQ = 0x05;
        public static readonly byte ACK = 0x06;
        public static readonly byte CR = 0x0D;
        public static readonly byte DC1 = 0x11;
        public static readonly byte DC2 = 0x12;
        public static readonly byte NAK = 0x15;
        public static readonly byte ESC = 0x1B;


        public static readonly byte PUT_RELEASE = 0x8d;
        public static readonly byte PUT_DATA = 0x85;

        public static readonly byte PUT_TRANSACTION_RESULT = 0x88;
        #endregion

        [Reactive] public float Reading { get; set; }
        [Reactive] public string ConsoleText { get; private set; }
        public ICommand GetReading { get; }
        public ICommand Disconnect { get; }

        IAdapter adapter;
        IDevice device;
        public IGattCharacteristic CharacteristicWrite { get; private set; }

        ModelType selectedModel;
        [Reactive] public bool scanEnabled { get; set; }
        public APWorkViewModel()
        {
            scanEnabled = false;

            allowedId.Add(knownId);
            allowedId.Add(new Guid("00000000-0000-0000-0000-cc81d47f7569 "));

            this.GetReading = ReactiveCommand.Create(() =>
            {
                try
                {
                    if (device != null && device.IsConnected() && !scanEnabled)
                    {
                        device.GetKnownCharacteristics(serviceguidHC, new Guid[] { wguid }).Subscribe(c =>
                        {
                            ConsoleOutput("characteristic:" + c.Uuid.ToString());
                            
                            if (c.CanNotify())
                            {
                                //enable notification
                                //c.EnableNotifications(true);
                                ConsoleOutput("notification is enabled");
                                c.RegisterAndNotify().Subscribe(result =>
                                {
                                    string data = new string(Encoding.UTF8.GetChars(result.Data));
                                    ProcessData(data);
                                });
                            }

                            if (c.CanWrite())
                            {
                                //send command
                                SendCommand(c);
                            }
                        });
                    }

                    if (scanEnabled)
                    {
                        //try to scan first
                        var scanner = this.adapter.Scan(
                            new ScanConfig
                            {
                                ServiceUuids = allowedId
                            }
                        )
                        .Subscribe(
                            results =>
                            {
                                this.adapter.StopScan();

                                knownId = results.Device.Uuid;
                                ConsoleOutput("found device" + results.Device.Name + " and stopped the scan");
                                ConnectDevice();
                            },
                            ex => ConsoleOutput("Scan Error:" + ex.ToString())
                        );
                    }
                }
                catch (Exception getDeviceEx)
                {
                    ConsoleOutput("GetReading Error:" + getDeviceEx.Message);
                }
            });

            this.Disconnect = ReactiveCommand.Create(() =>
            {
                try
                {
                    DisconnectDevice();
                }
                catch (Exception disEx)
                {
                    ConsoleOutput("Disconnect Error:" + disEx.Message);
                }
            });
        }
        
        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            this.adapter = parameters.GetValue<IAdapter>("adapter");
            this.selectedModel = parameters.GetValue<ModelType>("modelType");
            this.knownId = parameters.GetValue<Guid>("knownId");

            if (knownId.Equals(new Guid("00000000-0000-0000-0000-000000000000")))
            {
                scanEnabled = true;
                return;
            }

            ConnectDevice();
        }

        public override void OnDisappearing()
        {
            base.OnAppearing();
            DisconnectDevice();
        }

        private void ConnectDevice()
        {
            try
            {
                this.adapter.GetKnownDevice(knownId).Subscribe(knownDevice =>
                {
                    this.device = knownDevice;

                    this.device.Connect();
                    ConsoleOutput("DEVICE IS CONNECTED");
                    //this.device
                    //.WhenStatusChanged()
                    //.Subscribe(status =>
                    //{
                    //    switch (status)
                    //    {
                    //        case ConnectionStatus.Connecting:
                    //            break;
                    //        case ConnectionStatus.Connected:
                    //            ConsoleOutput("DEVICE IS CONNECTED");
                    //            break;
                    //        case ConnectionStatus.Disconnected:
                    //            ConsoleOutput("DEVICE IS DISCONNECTED");
                    //            break;
                    //    }
                    //});
                    //.DisposeWith(this.DeactivateWith);

                });
            }
            catch (Exception en)
            {
                ConsoleOutput("OnNav Error:" + en.Message);
            }
        }
        private void DisconnectDevice()
        {
            if (this.device != null)
            {
                this.device.CancelConnection();
            }
        }
        private void ProcessData(string dataString)
        {
            dataString = "DATA: " + dataString.Replace("\r", "\r\n");// + dataString3.Replace("\r", "\r\n");
            ConsoleOutput(dataString);
            //extract the reading
            switch (selectedModel)
            {
                case ModelType.Flowcom3000:
                    if (dataString.Contains("|"))
                    {
                        try
                        {
                            Reading = Convert.ToSingle(dataString.Split('|')[5].Trim());
                            //save the data to azure storage
                            Reading rdata = new Reading();
                            rdata.ReadingValue = Reading.ToString();
                            Task.Run(() => StorageHelper.Write(rdata));
                        }
                        catch { }
                    }
                    break;
                case ModelType.FlowcomS8:
                    if (dataString.Contains(","))
                    {
                        try
                        {
                            Reading = Convert.ToSingle(dataString.Split(',')[2].Trim());
                            //save the data to azure storage
                            Reading rdata = new Reading();
                            rdata.ReadingValue = Reading.ToString();
                            Task.Run(() => StorageHelper.Write(rdata));
                        }
                        catch { }
                    }
                    break;
            }
        }
        private void SendCommand(IGattCharacteristic wc)
        {
            //sending command
            byte[] cmd = new byte[] { };

            switch (selectedModel)
            {
                case ModelType.Flowcom3000:
                    cmd = CreateHeader(TXMODE, PUT_TRANSACTION_RESULT);//GetBytes(result.Text);
                    wc.Write(cmd);
                    break;
                case ModelType.FlowcomS8:
                    cmd = GetBytesForUTF8("L");
                    wc.Write(cmd);
                    break;
            }

            ConsoleOutput("command sent");
        }
        private byte[] CreateHeader(byte mode, byte opcode)
        {
            byte[] header = new byte[4];
            header[0] = EOT;
            header[1] = (byte)(mode | this.address);
            header[2] = opcode;
            header[3] = ENQ;
            return header;
        }

        private static byte[] GetBytesForUTF8(string text)
        {
            //return text.Split(' ').Where(token => !string.IsNullOrEmpty(token)).Select(token => Convert.ToByte(token, 16)).ToArray();
            return System.Text.Encoding.UTF8.GetBytes(text);
            //return System.Text.Encoding.Unicode.GetBytes(text);
        }
        private void ConsoleOutput(string msg)
        {
            ConsoleText += msg + "\r\n";
        }
    }
}
 