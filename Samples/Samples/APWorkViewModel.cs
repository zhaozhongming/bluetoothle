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
using System.Reactive.Linq;
using System.Linq;

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
        public ICommand StartScan { get; }
        public ICommand StopScan { get; }

        IAdapter adapter;
        IDisposable scan;
        IDevice device;
        public IGattCharacteristic CharacteristicWrite { get; private set; }

        bool IsNotifying;

        ModelType selectedModel;
        [Reactive] public bool ScanEnabled { get; set; }
        [Reactive] public bool IsScanning { get; private set; }
        [Reactive] public bool FoundDevice { get; set; }
        [Reactive] public string ScanButtonText { get; private set; }

        public APWorkViewModel()
        {
            ScanEnabled = false;
            allowedId.Add(knownId);
            allowedId.Add(new Guid("00000000-0000-0000-0000-cc81d47f7569 "));

            this.StartScan = ReactiveCommand.Create(() => 
            {
                try
                {
                    TryScan();
                }
                catch(Exception eScan)
                {
                    ConsoleOutput("扫描错误:" + eScan.Message);
                }
            });

            this.GetReading = ReactiveCommand.Create(() =>
            {
                try
                {
                    GetReadingForConnectedDevice();
                }
                catch (Exception getDeviceEx)
                {
                    ConsoleOutput("读取数据错误:" + getDeviceEx.Message);
                }
            });
        }
        
        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            this.adapter = parameters.GetValue<IAdapter>("adapter");
            this.selectedModel = parameters.GetValue<ModelType>("modelType");
            this.knownId = parameters.GetValue<Guid>("knownId");
            this.IsScanning = false;

            if (knownId.Equals(new Guid("00000000-0000-0000-0000-000000000000")))
            {
                ScanEnabled = true;
                TryScan();
                return;
            }

            ConnectDevice();
        }

        public override void OnDisappearing()
        {
            base.OnAppearing();
            DisconnectDevice();
        }

        private void TryScan()
        {
            if (ScanEnabled)
            {
                FoundDevice = false;

                if (this.IsScanning)
                {
                    ScanButtonText = "开始扫描";
                    this.scan?.Dispose();
                    this.IsScanning = false;
                }
                else
                {
                    ConsoleOutput("正在扫描设备......");
                    this.IsScanning = true;
                    ScanButtonText = "停止扫描";
                    //try to scan first
                    this.scan = this.adapter.Scan(
                    //new ScanConfig
                    //{
                    //    ServiceUuids = allowedId
                    //}
                    )
                    .Buffer(TimeSpan.FromSeconds(1))
                    .Timeout(TimeSpan.FromSeconds(5))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(
                        results =>
                        {
                            if (!FoundDevice)
                            {
                                var fs = (from d1 in results
                                          join d2 in allowedId
                                          on d1.Device.Uuid equals (Guid)d2
                                          select new { d1.Device.Uuid, d1.Rssi }).OrderBy(i => i.Rssi);

                                foreach (var fd in fs)
                                {
                                    ConsoleOutput("发现设备:" + fd.Uuid.ToString() + " " + fd.Rssi);
                                }

                                if (fs.Count() > 0)
                                {
                                    var foundDeviceId = fs.Last().Uuid;

                                    if (foundDeviceId != null)
                                    {
                                        this.scan?.Dispose();
                                        this.IsScanning = false;
                                        ScanButtonText = "开始扫描";
                                        ConsoleOutput("停止扫描");
                                        FoundDevice = true;
                                        knownId = foundDeviceId;
                                        ConsoleOutput("开始连接:" + foundDeviceId.ToString());
                                        ConnectDevice();
                                    }
                                }
                            }
                        },
                        ex => 
                        {
                            ConsoleOutput("扫描错误:" + ex.ToString());
                            this.scan?.Dispose();
                            this.IsScanning = false;
                            ScanButtonText = "开始扫描";
                            ConsoleOutput("停止扫描");
                        }
                    )
                    .DisposeWith(this.DeactivateWith);
                }
            }
        }
        private void ConnectDevice()
        {
            try
            {
                this.adapter.GetKnownDevice(knownId).Subscribe(knownDevice =>
                {
                    this.device = knownDevice;

                    this.device
                    .WhenStatusChanged()
                    .Subscribe(status =>
                    {
                        switch (status)
                        {
                            case ConnectionStatus.Connecting:
                                break;
                            case ConnectionStatus.Connected:
                                FoundDevice = true;
                                ConsoleOutput("设备已连接");
                                break;
                            case ConnectionStatus.Disconnected:
                                ConsoleOutput("设备已断开");
                                break;
                        }
                    })
                    .DisposeWith(this.DeactivateWith);

                    this.device.Connect();
                });
            }
            catch (Exception en)
            {
                ConsoleOutput("连接设备出错:" + en.Message);
            }
        }
        private void DisconnectDevice()
        {
            this.scan?.Dispose();
            this.IsScanning = false;

            if (this.device != null)
            {
                this.device.CancelConnection();
            }
        }

        private void GetReadingForConnectedDevice()
        {
            if (device != null && device.IsConnected())
            {
                device.GetKnownCharacteristics(serviceguidHC, new Guid[] { wguid }).Subscribe(c =>
                {
                    ConsoleOutput("发现特征:" + c.Uuid.ToString());

                    if (c.CanNotify() && !this.IsNotifying)
                    {
                        IsNotifying = true;
                        c.RegisterAndNotify().Subscribe(result =>
                        {
                            string data = new string(Encoding.UTF8.GetChars(result.Data));
                            ProcessData(data);
                        });
                        ConsoleOutput("数据通知服务已启用");
                    }

                    if (c.CanWriteWithoutResponse() || c.CanWriteWithResponse())
                    {
                        //send command
                        SendCommand(c);
                    }
                });
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
                            //Task.Run(() => StorageHelper.Write(rdata));
                            ConsoleOutput("数据读取并保存成功");
                            DisconnectDevice();
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
                            //Task.Run(() => StorageHelper.Write(rdata));
                            ConsoleOutput("数据读取并保存成功");
                            DisconnectDevice();
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
                    
                    break;
                case ModelType.FlowcomS8:
                    cmd = GetBytesForUTF8("L");
                    break;
            }

            if (wc.CanWriteWithResponse())
                wc.Write(cmd).Timeout(TimeSpan.FromSeconds(2))
                    .Subscribe(
                            x => ConsoleOutput("发送读取数据指令成功")
                        );
            else if (wc.CanWriteWithoutResponse())
                wc.WriteWithoutResponse(cmd).Timeout(TimeSpan.FromSeconds(2))
                    .Subscribe(
                            x => ConsoleOutput("发送单向读取数据指令成功")
                        );
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
 