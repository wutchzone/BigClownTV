using System;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace BigClownAppTV.Model
{
    public class Mqtt : MqttClient
    {
        public delegate void MessageRecievedEventHandler(object sender, MqttEventArgs e);

        public event MessageRecievedEventHandler MessageRecieved;
        public Mqtt(string hostname) : base(hostname, 1883, false, MqttSslProtocols.None)
        {
            base.Connect(Guid.NewGuid().ToString());
            base.Subscribe(new string[] { "nodes/bridge/0/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

            base.MqttMsgPublishReceived += OnMqttMsgPublishReceived;
            
        }

        private void OnMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs mqttMsgPublishEventArgs)
        {          
                JsonConverter a;
                Unit carry = null;
                Unit carry0 = null;
                string[] carry2 = mqttMsgPublishEventArgs.Topic.Split('/');

                try
                {
                    a = JsonConvert.DeserializeObject<JsonConverter>(Encoding.UTF8.GetString(mqttMsgPublishEventArgs.Message));
                }
                catch (Exception)
                {
                    Debug.WriteLine("Failed converting JSON to C# ");
                    return;                    
                }

                if (carry2[3] == "thermometer")
                {
                    carry = new Unit()
                    {
                        Value = float.Parse(a.temperature[0].ToString()),
                        Label = "°C",
                        Time = DateTime.UtcNow,
                        Header = "Thermometer"
                    };
                }
                else if (carry2[3] == "lux-meter")
                {
                    carry = new Unit()
                    {
                        Value = float.Parse(a.illuminance[0].ToString()),
                        Label = a.illuminance[1].ToString(),
                        Time = DateTime.UtcNow,
                        Header = "Lux meter"
                    };
                }
                else if (carry2[3] == "humidity-sensor")
                {
                    carry = new Unit()
                    {
                        Value = float.Parse(a.relativehumidity[0].ToString()),
                        Label = a.relativehumidity[1].ToString(),
                        Time = DateTime.UtcNow,
                        Header = "Humidity sensor"
                    };
                    
                }
                else if (carry2[3] == "barometer")
                {
                    carry = new Unit()
                    {
                        Value = float.Parse(a.altitude[0].ToString()),
                        Label = a.altitude[1].ToString(),
                        Time = DateTime.UtcNow,
                        Header = "Barometer/altitude"
                    };
                    carry0 = new Unit()
                    {
                        Value = float.Parse(a.pressure[0].ToString()),
                        Label = a.pressure[1].ToString(),
                        Time = DateTime.UtcNow,
                        Header = "Barometer/pressure"
                    };

                }

                if (carry != null)
                {
                    MessageRecieved?.Invoke(this,
                        new MqttEventArgs()
                        { Unit = carry });
                }
                if (carry0 != null)
                {
                    MessageRecieved?.Invoke(this,
                        new MqttEventArgs()
                        { Unit = carry0 });
                }
        }

        /// <summary>
        /// Event args can be specified
        /// </summary>
        public class MqttEventArgs
        {
            public Unit Unit { get; set; }
        }

        private class JsonConverter
        {
            public object[] temperature { get; set; }

            public object[] illuminance { get; set; }

            [JsonProperty("relative-humidity")]
            public object[] relativehumidity { get; set; }

            public object[] altitude { get; set; }
            public object[] pressure { get; set; }
        }
    }
}
