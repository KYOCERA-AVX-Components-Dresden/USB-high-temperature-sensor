using System;

namespace USB_data_logger.Models
{
    /// <summary>
    /// this object contains the measurement data of the sensor
    /// </summary>
    public class DataMeasurementSensor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="outputRawData">Temperature raw data including CRC</param>
        /// <param name="output">Temperature value in °C</param>
        /// <param name="outputTimeStamp">Time stamp of the temperature value</param>
        public DataMeasurementSensor(string outputRawData, double output, DateTime outputTimeStamp)
        {
            OutputRawData = outputRawData;
            Output = output;
            OutputTimeStamp = outputTimeStamp;
        }

        /// <summary>
        /// Temperature raw data including CRC
        /// </summary>
        public readonly string OutputRawData;
        
        /// <summary>
        /// Temperature value in °C
        /// </summary>
        public readonly double Output;
        
        /// <summary>
        /// Time stamp of the temperature value
        /// </summary>
        public readonly DateTime OutputTimeStamp;
    }
}
