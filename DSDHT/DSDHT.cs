using System;
using System.Collections.Generic;
using System.Linq;
using Sensors.Dht;
using Sensors.OneWire.Common;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sensors.OneWire
{
    public class DSDHT
    {
        GpioPin _pin = null;
        private IDht _dht = null;
        private List<int> _retryCount = new List<int>();
        private DateTimeOffset _startedAt = DateTimeOffset.MinValue;
        private bool init = false;

        private float _humidity = 0f;
        public float Humidity
        {
            get
            {
                return _humidity; //(0)
            }

            set
            {
                _humidity = value;
            }
        }

        public string HumidityDisplay
        {
            get
            {
                return string.Format("{0:0.0}% RH", this.Humidity);
            }
        }

        private float _temperature = 0f;
        public float Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
            }
        }

        public string TemperatureDisplay
        {
            get
            {
                return string.Format("{0:0.0} °C", this.Temperature);
            }
        }

        public DSDHT()
        {

            GpioController controller = GpioController.GetDefault();

            if (controller != null)
            {
                _pin = GpioController.GetDefault().OpenPin(4, GpioSharingMode.Exclusive);
                _dht = new Dht11(_pin, GpioPinDriveMode.Input);
                _startedAt = DateTimeOffset.Now;
                init = true;
            }
        }

        public async Task DSgetValueAsync()
        {
            DhtReading reading = new DhtReading();

            reading = await _dht.GetReadingAsync().AsTask();

            if (reading.IsValid)
            {
                this.Temperature = Convert.ToSingle(reading.Temperature);
                this.Humidity = Convert.ToSingle(reading.Humidity);//(3)

            }
        }

        public async Task DSgetTemperatureAsync()
        {
            DhtReading reading = new DhtReading();

            reading = await _dht.GetReadingAsync().AsTask();

            if (reading.IsValid)
            {
                this.Temperature = Convert.ToSingle(reading.Temperature);
            }
        }

        public async Task DSgetHumidityAsync()
        {
            DhtReading reading = new DhtReading();

            reading = await _dht.GetReadingAsync().AsTask();

            if (reading.IsValid)
            {
                this.Temperature = Convert.ToSingle(reading.Temperature);
            }
        }

        public void WhenStop()
        {
            if (_pin != null)
            {
                _pin.Dispose();
                _pin = null;
            }

            // ***
            // *** Set the Dht object reference to null.
            // ***
            _dht = null;

            // ***
            // *** Stop the high CPU usage simulation.
            // ***
            CpuKiller.StopEmulation();
        }

        public bool IsInit()
        {
            return init;
        }

    }
}
