using System;
using System.Diagnostics;
using USB_data_logger.Calculations;
using USB_data_logger.Models;


namespace USB_data_logger.Pheripherals
{
    public class UsbHighTemperatureSensor
    {

        /// <summary>
        /// Com port or serial port for communication between computer and sensor
        /// </summary>
        private System.IO.Ports.SerialPort? _SerialPort;

        /// <summary>
        /// this method initializes the serial port and the temperature sensor
        /// </summary>
        /// <param name="portName">name of the serial port, e.g. COM1</param>
        /// <param name="message">error message in case of initialization issue</param>
        /// <returns>true -> communication successful established; false -> sometime went wrong, see message for details</returns>
        public bool Init(string portName, ref string message)
        {
            //#00 define vars
            const int BAUDRATE = 9600; //Baud rate is defined by Sensor firmware
            DataInformationSensor info = null;
            var rslt = false;

            //#01 try to open serial port 
            try
            {
                //hint serial port configuration based on sensor manual
                _SerialPort = new System.IO.Ports.SerialPort();
                _SerialPort.PortName = portName;
                _SerialPort.BaudRate = BAUDRATE; //Baud rate define by manual, do not change
                _SerialPort.DataBits = 8;
                _SerialPort.StopBits = System.IO.Ports.StopBits.One;
                _SerialPort.NewLine = "\r"; //\r define by manual
                _SerialPort.ReadTimeout = 500;
                _SerialPort.Open();

                //#02 request sensor information to test communication
                rslt = GetSensorInformation(ref info);

                if (!rslt) message = "No feedback from sensor during initialization process.";
            }
            catch (Exception ex)
            {
                message = $"Serial port ({portName}) could n´t be initialized. \nError message: {ex.Message}";
            }

            //#3 if communication test failed, serial port will be closed
            if (!rslt)
            {
                if (_SerialPort != null)
                {
                    _SerialPort.Close();
                    _SerialPort.Dispose();
                }
            }

            return rslt;
        }

        public void Dispose()
        { 
            if(_SerialPort!=null) _SerialPort.Dispose();   
        }

        /// <summary>
        /// this method requests the temperature of the sensor tip
        /// </summary>
        /// <returns>true -> everything is fine; false -> something went wrong</returns>
        public bool GetSensorOutput(ref DataMeasurementSensor? dataSensor)
        { 
            //#0 define vars
            var rslt = false;
            var response = "";
            const string TEMP_REQ = "T?"; //command according product description
            CalcHelper_UsbSensor calcHelper = new CalcHelper_UsbSensor();

            //#2 request data from sensor
            if (RequestData(TEMP_REQ, ref response))
            { 
               //#3 process and check response
               rslt = calcHelper.ConvertSensorOutput(response, ref dataSensor);
            }       

            //#4 return results
            return rslt;
        }

        /// <summary>
        /// this method requests the sensor information
        /// </summary>
        /// <param name="dataInformationSensor">class with convert sensor information data</param>
        /// <returns></returns>
        public bool GetSensorInformation(ref DataInformationSensor? dataInformationSensor) 
        {
            //#0 define vars
            var rslt = false;
            var response = "";
            const string INFO_REQ = "I?"; //command according product description
            CalcHelper_UsbSensor calcHelper = new CalcHelper_UsbSensor();

            //#2 request data from sensor
            if (RequestData(INFO_REQ, ref response))
            {
                //#3 process and check response
                rslt = calcHelper.ConvertSensorInformation(response, ref dataInformationSensor);
            }

            //#4 return results
            return rslt;
        }

        /// <summary>
        /// this method requests data from the sensor 
        /// </summary>
        /// <param name="command">command which should by transmitted</param>
        /// <param name="response">response of the sensor, if response just nothing something went wrong</param>
        /// <returns>true -> everything was fine, false -> something went wrong</returns>
        private bool RequestData(string command, ref string response)
        {
            //#00 define vars
            var rslt =false;
            
            try
            {  
                //#01 send request string
                _SerialPort.WriteLine(command);  

                //#02 read response
                response = _SerialPort.ReadLine();
                rslt = true;
            }
            catch (Exception ex)
            {
                //error output just in DEBUG mode available
                #if DEBUG
                    Debug.WriteLine($"Error occurred during command {command}. Error description: {ex.Message}");
                #endif
            }

            //03 return results
            return rslt;
        }
    }
}
