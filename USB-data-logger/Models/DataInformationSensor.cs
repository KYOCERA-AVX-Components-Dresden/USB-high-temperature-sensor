
namespace USB_data_logger.Models
{
    /// <summary>
    /// this object contains information about the sensor
    /// </summary>
    public class DataInformationSensor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rawData">Information raw data string including the crc</param>
        /// <param name="type">Type of the sensor</param>
        /// <param name="unit">Output unit of the sensor output</param>
        /// <param name="outputMin">Minimum output value of the sensor</param>
        /// <param name="outputMax">Maximum output value of the sensor</param>
        /// <param name="firmwareVersion">Firmware version of the sensor firmware</param>
        /// <param name="hardwareVersion">Hardware version of the sensor hardware</param>
        public DataInformationSensor(string rawData, string type,string unit, double outputMin, double outputMax, int firmwareVersion, int hardwareVersion )
        {
            RawData = rawData;
            Type = type;
            FirmwareVersion = firmwareVersion;
            HardwareVersion = hardwareVersion;
            Unit = unit;
            OutputMin = outputMin;
            OutputMax = outputMax;
        }

        /// <summary>
        /// Information raw data string including the crc
        /// </summary>
        public readonly string RawData;

        /// <summary>
        /// Type of the sensor
        /// </summary>
        public readonly string Type;

        /// <summary>
        /// Firmware version of the sensor firmware
        /// </summary>
        public readonly int FirmwareVersion;

        /// <summary>
        /// Hardware version of the sensor hardware
        /// </summary>
        public readonly int HardwareVersion;

        /// <summary>
        /// Output unit of the sensor output
        /// </summary>
        public readonly string Unit;

        /// <summary>
        /// Minimum output value of the sensor
        /// </summary>
        public readonly double OutputMin;

        /// <summary>
        /// Maximum output value of the sensor
        /// </summary>
        public readonly double OutputMax;

    }
}
