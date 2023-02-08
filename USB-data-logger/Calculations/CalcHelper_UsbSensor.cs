using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using USB_data_logger.Models;

namespace USB_data_logger.Calculations
{
    public class CalcHelper_UsbSensor
    {
        /// <summary>
        /// this method extracts the crc of a sensor response
        /// </summary>
        /// <param name="rawData">response string of the sensor, without CR</param>
        /// <returns>CRC value as integer</returns>
        public int GetCrc(string rawData)
        {
            //#00 define vars
            int rslt = 0;

            //#01 check format of data
            if (rawData.Contains(":"))
            {
                //#2 extract crc
                string[] parts = rawData.Split(':');
                string crcString = parts[parts.Length - 1];

                Int32.TryParse(crcString,
                    System.Globalization.NumberStyles.HexNumber,
                    System.Globalization.CultureInfo.InvariantCulture, 
                    out rslt);
            }

            return rslt;
        }

        /// <summary>
        /// this method converts the received data from the sensor to the data model
        /// </summary>
        /// <param name="rawData">received data string from the sensor</param>
        /// <param name="data">data model with temperature data</param>
        /// <returns>true -> conversation succeed; false -> conversation failed</returns>
        public bool ConvertSensorOutput(string rawData, ref DataMeasurementSensor? data)
        {
            //#0 define vars
            bool rslt = false;

            //#1 check crc
            if (CheckCrc(rawData))
            {  
                //#2 convert data
                string[] parts = rawData.Split(":");
                double output = 0;

                if (double.TryParse(parts[0], 
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, 
                                out output))
                {
                    data = new DataMeasurementSensor(rawData, output, System.DateTime.Now);
                    rslt = true;
                }
            }

            //output just in debug mode
            if (!rslt)
            {
                #if DEBUG
                      Debug.WriteLine($"Error occurred during temperature string conversation. Response Sensor: {rawData}");
                #endif
            }

            //#3 return results
            return rslt;
          }

        /// <summary>
        /// this method check the CRC of the received data
        /// </summary>
        /// <param name="rawData">received data from the sensor</param>
        /// <returns>true -> CRC correct; false -> CRC wrong</returns>
        public bool CheckCrc(string rawData)
        {
            //0 define vars
            bool rslt = false;
            int crcOld = 0;
            byte crcNew = 0;

            //1 get transmitted CRC
            crcOld = GetCrc(rawData);

            //2 calculate CRC based on string
            if (CalcCrc(rawData, ref crcNew))
            {
                //3 compare CRC
                rslt = crcNew == crcOld;
            }      

            //4 return results
            return rslt;
        }

        /// <summary>
        /// this method calculates the CRC of the received data
        /// </summary>
        /// <param name="rawData">received data from the sensor</param>
        /// <param name="crc">calculated CRC of the raw data</param>
        /// <returns>true -> CRC calculation succed; false -> CRC calculation failed</returns>
        public bool CalcCrc(string rawData, ref byte crc)
        {
            //0 define vars
            bool rslt = false;

            //2 calculate CRC based on string
            if (rawData.Contains(":"))
            {
                char[] charArray = rawData.ToCharArray();
                int posLastDoublePoint = rawData.LastIndexOf(":");

                //#2.1 sum up cars
                for (int i = 0; i < posLastDoublePoint; i++)
                {
                    crc += (byte)charArray[i];
                }

                rslt = true;
            }
            
            //#3 return result
            return rslt; 
        }


        /// <summary>
        /// this method convert the sensor information into the data model
        /// </summary>
        /// <param name="rawData">received data from the sensor</param>
        /// <param name="data">converted data</param>
        /// <returns>true -> conversation succeed; false conversation failed</returns>
        public bool ConvertSensorInformation(string rawData, ref DataInformationSensor data)
        {
            //#0 define vars
            bool rslt = false;
            const string DIVIDER = ":";
        
            //#1 try to convert response
            if (rawData.Contains(DIVIDER))
            {
                //check crc
                if (CheckCrc(rawData))
                {
                    string[] split = rawData.Split('\n');
                    if (split.Length > 8)
                    {
                        try
                        {
                            NumberFormatInfo provider = new NumberFormatInfo();
                            data = new DataInformationSensor(
                                rawData,
                                split[0].Split(DIVIDER)[1], //Type
                                split[1].Split(DIVIDER)[1], //Unit
                                Convert.ToDouble(split[2].Split(DIVIDER)[1], provider), //Output min
                                Convert.ToDouble(split[3].Split(DIVIDER)[1], provider), //Output max
                                Convert.ToInt32(split[6].Split(DIVIDER)[1]),  //Firmware version
                                Convert.ToInt32(split[7].Split(DIVIDER)[1])); //Hardware version
                            rslt = true;
                        }
                        catch (Exception ex)
                        {
                            #if DEBUG
                                Debug.WriteLine($"Error occurred during sensor information string conversation. Response Sensor: {rawData}");
                            #endif
                        }
                    }
                }
                else
                {
                    #if DEBUG
                         Debug.WriteLine($"CRC check failed during sensor information string conversation. Response Sensor: {rawData}");
                    #endif
                }
            }
            
            //#2 return results
            return rslt;               
        }
    }
}
