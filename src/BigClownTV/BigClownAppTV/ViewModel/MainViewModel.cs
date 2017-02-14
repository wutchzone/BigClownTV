using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Core;
using BigClownAppTV.Annotations;
using BigClownAppTV.Model;
using BigClownAppTV.Base.Commands;

namespace BigClownAppTV.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Interface implementation
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
                );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        }
        #endregion

        /// <summary>
        /// Local variables.
        /// </summary>
        #region Local variables
        private Mqtt _mqtt;
        private DB _db;
        private GraphQueue<Unit> _queue;
        private Queue<Unit> _thermometer; 
        private Queue<Unit> _lux_meter;
        private Queue<Unit> _humidity_sensor;
        private Queue<Unit> _barometer_altitude;
        private Queue<Unit> _barometer_pressure;

        private ObservableCollection<Unit> _unitCollection;
        private bool _isLoading;
        private string _debugLog;
        private string _ipAddress;
        private string _graphHeader;
        private string _graphValue;

        #endregion

        /// <summary>
        /// They are used for binding from the view (XAML).
        /// </summary>
        #region Public variables
        public ObservableCollection<Unit> UnitCollection { get { return _unitCollection; } set { _unitCollection = value; OnPropertyChanged(); } }
        public bool IsLoading { get { return _isLoading; } set { _isLoading = value; OnPropertyChanged(); } }
        public string DebugLog { get { return _debugLog; } set { _debugLog = value; OnPropertyChanged(); } }
        public string IpAddress { get { return _ipAddress; } set { _ipAddress = value; OnPropertyChanged(); } }
        public string GraphHeader { get { return _graphHeader; } set { _graphHeader = value; OnPropertyChanged(); } }
        public string GraphValue { get { return _graphValue; } set { _graphValue = value; OnPropertyChanged(); } }
        #endregion

        /// <summary>
        /// Commands used to execute some specific code written in ViewModel.
        /// </summary>
        #region Commands
        public ConnectCommand Connect { get; set; }
        #endregion 

        /// <summary>
        /// Init basic objects
        /// </summary>
        public MainViewModel()
        {
            //_db = new DB();
            
            _thermometer = new Queue<Unit>();
            _lux_meter = new Queue<Unit>();
            _humidity_sensor = new Queue<Unit>();
            _barometer_altitude = new Queue<Unit>();
            _barometer_pressure = new Queue<Unit>();

            _queue = new GraphQueue<Unit>(10, 86399, _thermometer, _lux_meter, _humidity_sensor, _barometer_altitude, _barometer_pressure);

            Connect = new ConnectCommand(BrokerConnection);

            StartSetup();
        }

        async void StartSetup()
        {

            //UnitCollection = new ObservableCollection<Unit>(_thermometer);  //(_queue.Peek() as ObservableCollection<Unit>);

            _queue.GraphHandler += (sender, args) =>
            {
                UnitCollection = new ObservableCollection<Unit>(args.List);
                GraphHeader = args.Header;
                GraphValue = args.Label;
            };
#region hide
            /*
            await Task.Run(() =>
            {
                foreach (var a in _db.Table<DB.Temperature>())
                {
                    if (a.Time < DateTime.UtcNow.AddMinutes(-1)) continue;
                    _thermometer.Add(new Unit() { Header = a.Header, Label = a.Label, Time = a.Time, Value = a.Value });
                    System.Diagnostics.Debug.WriteLine("Temperature" + a.Id);

                }
            });

            await Task.Run(() =>
            {
                foreach (var a in _db.Table<DB.Altitude>())
                {
                    if (a.Time < DateTime.UtcNow.AddMinutes(-1)) continue;
                    _barometer_altitude.Add(new Unit()
                    {
                        Header = a.Header,
                        Label = a.Label,
                        Time = a.Time,
                        Value = a.Value
                    });
                    System.Diagnostics.Debug.WriteLine("Altitude");
                }
            });

            await Task.Run(() =>
            {
                foreach (var a in _db.Table<DB.Humidity>())
                {
                    if (a.Time < DateTime.UtcNow.AddMinutes(-1)) continue;
                    _humidity_sensor.Add(new Unit() { Header = a.Header, Label = a.Label, Time = a.Time, Value = a.Value });
                    System.Diagnostics.Debug.WriteLine("Humidity");
                }
            });

            await Task.Run(() =>
            {
                foreach (var a in _db.Table<DB.Lux>())
                {
                    if (a.Time < DateTime.UtcNow.AddMinutes(-1)) continue;
                    _lux_meter.Add(new Unit() { Header = a.Header, Label = a.Label, Time = a.Time, Value = a.Value });
                    System.Diagnostics.Debug.WriteLine("Lux");
                }
            });

            await Task.Run(() =>
            {
                foreach (var a in _db.Table<DB.Pressure>())
                {
                    if (a.Time < DateTime.UtcNow.AddMinutes(-1)) continue;
                    _barometer_pressure.Add(new Unit()
                    {
                        Header = a.Header,
                        Label = a.Label,
                        Time = a.Time,
                        Value = a.Value
                    });
                    System.Diagnostics.Debug.WriteLine("Pressure");
                }
            });*/
#endregion
            System.Diagnostics.Debug.WriteLine("Hotovo");            
        }

        void ConfigureMqtt()
        {
            _mqtt.MessageRecieved += async (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine("Message accepted from MQTT");
                if (args.Unit.Label == "°C")
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        () =>
                        {
                            _thermometer.Enqueue(args.Unit);                           
                        });
                    //_db.Insert(new DB.Temperature()
                    //{
                    //    Header = args.Unit.Header,
                    //    Value = args.Unit.Value,
                    //    Label = args.Unit.Label,
                    //    Time = args.Unit.Time
                    //});
                }
                else if (args.Unit.Label == "lux")
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        () =>
                        {
                            _lux_meter.Enqueue(args.Unit);
                        });
                    //_db.Insert(new DB.Lux()
                    //{
                    //    Header = args.Unit.Header,
                    //    Value = args.Unit.Value,
                    //    Label = args.Unit.Label,
                    //    Time = args.Unit.Time
                    //});
                }
                else if (args.Unit.Label == "%")
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        () =>
                        {
                            _humidity_sensor.Enqueue(args.Unit);
                        });
                    //_db.Insert(new DB.Humidity()
                    //{
                    //    Header = args.Unit.Header,
                    //    Value = args.Unit.Value,
                    //    Label = args.Unit.Label,
                    //    Time = args.Unit.Time
                    //});
                }
                else if (args.Unit.Label == "m")
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        () =>
                        {
                            _barometer_altitude.Enqueue(args.Unit);
                        });
                    //_db.Insert(new DB.Altitude()
                    //{
                    //    Header = args.Unit.Header,
                    //    Value = args.Unit.Value,
                    //    Label = args.Unit.Label,
                    //    Time = args.Unit.Time
                    //});
                }
                else if (args.Unit.Label == "kPa")
                {
                   await  Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        () =>
                        {
                            _barometer_pressure.Enqueue(args.Unit);
                        });
                    //_db.Insert(new DB.Pressure()
                    //{
                    //    Header = args.Unit.Header,
                    //    Value = args.Unit.Value,
                    //    Label = args.Unit.Label,
                    //    Time = args.Unit.Time
                    //});                   
                }
            };
        }

        /// <summary>
        /// Method connected with ConnectComand.
        /// </summary>
        /// <param name="status">Returns enum depends on connection status.</param>
        async void BrokerConnection(ConnectCommand.ConnectStatus status)
        {
            if (status == ConnectCommand.ConnectStatus.Connect)
            {
                if (string.IsNullOrEmpty(this.IpAddress)) return;

                IsLoading = true;


                _mqtt = await ConnectIt();
            }



            else if (status == ConnectCommand.ConnectStatus.Disconnect)
            {
                IsLoading = true;
                try
                {
                    _mqtt.Disconnect();
                }
                catch (Exception)
                {

                    Debug("Already disconnected.");
                    return;

                }

                _mqtt = null;
                IsLoading = false;
                Debug("Disconnecting...");
            }

        }

        Task<Mqtt> ConnectIt()
        {
            var x = Task.Factory.StartNew(() =>
             {

                 try
                 {
                     _mqtt = new Mqtt(IpAddress);
                     if (_mqtt.IsConnected)
                     {
                         ConfigureMqtt();
                         Debug(string.Format("Connected to {0}", IpAddress));
                     }
                 }
                 catch (Exception)
                 {
                     Debug("Error with connection");

                     _mqtt = null;
                 }
                 IsLoading = false;
                 return _mqtt;
             });
            return x;
        }

        /// <summary>
        /// Additional method to write some text to Debug TextBlock in SettingsPage.
        /// </summary>
        /// <param name="text"></param>
        void Debug(string text)
        {
            DebugLog += text + '\n';
        }
    }
}
