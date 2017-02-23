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

            _thermometer = new Queue<Unit>();
            _lux_meter = new Queue<Unit>();
            _humidity_sensor = new Queue<Unit>();
            _barometer_altitude = new Queue<Unit>();
            _barometer_pressure = new Queue<Unit>();

            //86399
            _queue = new GraphQueue<Unit>(10, 28800, _thermometer, _humidity_sensor, _barometer_altitude, _barometer_pressure, _lux_meter);

            Connect = new ConnectCommand(BrokerConnection);

            IpAddress = "10.0.0.40";

            _queue.GraphHandler += (sender, args) =>
            {
                UnitCollection = new ObservableCollection<Unit>(args.List);
                GraphHeader = args.Header;
                GraphValue = args.Label;

            };

            Connect.Execute(ConnectCommand.ConnectStatus.Connect);
        }

        void ConfigureMqtt()
        {
            _mqtt.MessageRecieved += (sender, args) =>
            {
                        if (args.Unit.Label == "°C")
                        {
                            _thermometer.Enqueue(args.Unit);

                        }
                        else if (args.Unit.Label == "lux")
                        {
                            _lux_meter.Enqueue(args.Unit);
                        }
                        else if (args.Unit.Label == "%")
                        {
                            _humidity_sensor.Enqueue(args.Unit);
                        }
                        else if (args.Unit.Label == "m")
                        {
                            _barometer_altitude.Enqueue(args.Unit);
                        }
                        else if (args.Unit.Label == "kPa")
                        {
                            _barometer_pressure.Enqueue(args.Unit);
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
