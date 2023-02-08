# USB high temperature sensor

Our well known thermo couple high temperature sensor has been given an USB Interface. This new high temperature sensor has a temperature measurement range from -40°C up to 1200°C and a resolution of 0.1K. The communication and power supply of the whole sensor is provided completely by an USB Mini cable. The sensor is designed for general industry applications, test laboratories or smart home applications. The implementation of the sensor in the final application takes place by a virtual serial port and simple control commands.

## Data logger demo app in C#

We created an easy use data logger in C# to provide an easy and fast implementation of USB high temperature sensor in your target application.
<br><br>You can find the Git Hub Repository here ➞ [Git Hub Repository](https://github.com/BorisBloxsberg73/USB-high-temperature-sensor/tree/main)

## Driver

The sensor is using a virtual serial port or virtual comport (VCP). The driver for the virtual comport will be depolyed by the windows update service. If you are using an other OS or your computer has no access to the internet, you can downlod the driver also directly from the FTDI website ➞ [FTDI driver download page](https://ftdichip.com/drivers/vcp-drivers/)

## Commands

The control commands for the sensor are simple. The sensor has just two commands. Please find more details in the table below. Every command must be finisch by an carriage return ("\r"). If you don´t send this sign, the sensor will not respond. The sensor will finishing his response also with a carriage return ("\r"). <br> <br> (**Note**: The CRC calculation is descripted in the next section.)

<table border="2" bordercolor="#ff0000">
    <thead>
        <tr>
            <th width="10%">Command</th>
            <th width="45%">Command description</th>
            <th width="45%">Examble for response</th>     
        </tr>
    </thead>
    <tbody>
        <tr>
            <td align="center">T?</td>
            <td align="left">Request of the current temperature of the sensor tip. The unit of the response is Celsius (°C) and won´t be transmitted.</td>
            <td align="left">+545.4:2BCR (CR means carrige return)</td>
        </tr>
        <tr>
            <td align="center">I?</td>
            <td align="left">Request of the sensor information string. This response containing a lot of detailed static informations about the sensor e.g. Firmware version, output range. The response string is very long and needs some time to be transmitted from the sensor to the computer.</td>
            <td align="left">“Type:USB high temperature sensorLFOutput unit:CLF...CR<br><br>(<b>Note:</b> CR means carrige return, LF means line feed) </td>
        </tr>
    </tbody>
</table>


### CRC 

The sensor response containing always an check sum. This check sum is used to prevent wrong data transmission or data manipulation. The CRC is separated by a double point from the sensor data and is placed at the end of the sensor response. Please find below more details and a calculation example.

#### CRC calculation algorithm

The CRC calculation algorithm sum up only the decimal values of the ASCII signs in front of the double point and adds this to the payload. It will be separated by a double point from the payload. 
<table border="2" bordercolor="#ff0000">
    <thead>
        <tr>
            <th>Reponse from the sensor after T? command</th>
            <th>+</th>
            <th>5</th>
            <th>4</th>
            <th>5</th>
            <th>.</th>
            <th>4</th>
            <th>:</th>
            <th>2</th>
            <th>B</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>Decimal value of the ASCII sign</td>
            <td align="center">43</td>
            <td align="center">53</td>
            <td align="center">52</td>
            <td align="center">53</td>
            <td align="center">46</td>
            <td align="center">52</td>
            <td colspan=3 align="center"> not relevant for calculation</td>
        </tr>
        <tr>
            <td>Calculation algorithm</td>
            <td colspan=9>Sum up of the decimal values: 43+53+52+53+46+52 = 44 -> 0x2B
           <br> <b>Note:</b> after 43+53+52+53+46+52 the 8 - bit var does overflow. So the Result turns to 44 and not 299 </td>
        </tr>
    </tbody>
</table>

#### CRC calculation code example


```csharp
/// <summary>
/// this method calculates the CRC of the received data
/// </summary>
/// <param name="rawData">received data from the sensor</param>
/// <param name="crc">calculated CRC of the raw data</param>
/// <returns>true -> CRC calculation succeed; false -> CRC calculation failed</returns>
public bool CalcCrc(string rawData, ref byte crc)
{
    //0 define vars
    bool rslt = false;

    //2 calculate CRC based on string
    if (rawData.Contains(":"))
    {
        char[] charArray = rawData.ToCharArray();
        int posLastDoublePoint = rawData.LastIndexOf(":");

        //#2.1 sum up chars
        for (int i = 0; i < posLastDoublePoint; i++)
        {
            crc += (byte)charArray[i];
        }

        rslt = true;
    }

    //#3 return result
    return rslt; 
}
```

## Serial port setup

Please find below a C# code example for the serial port setup. 
**Note:** The name of the port depends on your system. Please double check your device manager for real portname because If you connect an other USB device the comport name could be changed.

```csharp
_SerialPort = new System.IO.Ports.SerialPort();
_SerialPort.PortName = "COM1"; //hint: "COM1" is just an examble
_SerialPort.BaudRate = 9600; //Baud rate is defined by the sensor, do not change
_SerialPort.DataBits = 8;
_SerialPort.StopBits = System.IO.Ports.StopBits.One;
_SerialPort.NewLine = "\r"; //Baud rate is defined by the sensor, do not change
_SerialPort.ReadTimeout = 500;
```

## FAQ and trouble shotting

- **What type of USB protocol uses the sensor?**
  - The sensor uses USB 2.0.
- **What type of USB connector does the sensor use?**
  - The sensor uses an USB Mini connector.
- **Does the sensor require an external power supply?**
  - No, the sensor is bus powered.
- **Is it possible to connect more than one sensor to my computer?**
  - Yes, it is. You can connect more than one sensor to a computer. Every sensor uses an own serial port.
- **Is there an other app to communicate with the sensor?**
  - Yes, you could use every terminal program for the serial port e.g. HTerm or the Hyperterminal
  
- **I can not select a comport in the data logger app.**
  - Please check the USB connection between the sensor and computer
  - Change the USB port
  - Use the USB ports at the backside of the computer
  - Disconnect the sensor wait one minute, connect the sensor, wait again one minute and try again
  - Check your device manager after unkown devices

- **I do not get a response from the sensor.**
  - Please check your command. 
  - Please only use capital letters for the command string and donn´t insert space in the command.
  - Check your command regarding the carriage return. The senosr needs the carriage return to find the command end.
  - Check the baudrate of the serial port. The required baudrate is 9600 kbits.

- **I just get a crypitc response from the sensor**
  - Check the Baudrate of the serial port. The required baudrate is 9600 kbits.
