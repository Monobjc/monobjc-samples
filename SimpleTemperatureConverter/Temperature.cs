using System;
using Monobjc.Foundation;

namespace Monobjc.Samples.SimpleTemperatureConverter
{
    [ObjectiveCClass]
    public class Temperature : NSObject
    {
        private double celsius;

        /// <summary>
        /// Initializes a new instance of the <see cref="Temperature"/> class.
        /// </summary>
        public Temperature() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Temperature"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public Temperature(IntPtr nativePointer)
            : base(nativePointer) {}

        /// <summary>
        /// Since our storage is celsius, and the other values are just computed, we declare that a change in celsius causes change in fahrenheit and kelvin values.
        /// </summary>
        [ObjectiveCMessage("initialize")]
        public new static void Initialize()
        {
            Class cls = Class.Get(typeof (Temperature));
            cls.SendMessage("setKeys:triggerChangeNotificationsForDependentKey:",
                            NSArray.ArrayWithObject((NSString) "celsius"), (NSString) "fahrenheit");
            cls.SendMessage("setKeys:triggerChangeNotificationsForDependentKey:",
                            NSArray.ArrayWithObject((NSString) "celsius"), (NSString) "kelvin");
        }

        /// <summary>
        /// Methods to set/get the celsius value simply work in terms of the stored value.  Strictly speaking, these two methods are not necessary, since the celsius value can be set/get with key value coding, that is, with setValue:forKey: and valueForKey:. However, having explicit methods that take the right kind of argument provide a better programming experience, both internally to this class and for any clients.
        /// </summary>
        /// <value>The celsius.</value>
        public double Celsius
        {
            [ObjectiveCMessage("celsius")]
            get { return this.celsius; }
            [ObjectiveCMessage("setCelsius:")]
            set
            {
                this.WillChangeValueForKey("celsius");
                this.celsius = value;
                this.DidChangeValueForKey("celsius");
            }
        }

        /// <summary>
        /// These methods simply set the stored value in celsius based on the incoming kelvin or fahrenheit value, or get the value from celsius.
        /// </summary>
        public double Kelvin
        {
            [ObjectiveCMessage("kelvin")]
            get { return this.Celsius + 273.15; }
            [ObjectiveCMessage("setKelvin:")]
            set { this.Celsius = value - 273.15; }
        }

        /// <summary>
        /// These methods simply set the stored value in celsius based on the incoming kelvin or fahrenheit value, or get the value from celsius.
        /// </summary>
        public double Fahrenheit
        {
            [ObjectiveCMessage("fahrenheit")]
            get { return this.Celsius*1.8 + 32; }
            [ObjectiveCMessage("setFahrenheit:")]
            set { this.Celsius = (value - 32)/1.8; }
        }
    }
}