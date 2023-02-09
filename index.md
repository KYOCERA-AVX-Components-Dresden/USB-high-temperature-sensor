---
layout: default
title:
---
# USB high temperature sensor

Our well known thermo couple high temperature sensor has been given an USB Interface. This new high temperature sensor has a temperature measurement range from -40°C up to 1200°C and a resolution of 0.1K. The communication and power supply of the whole sensor is provided completely by an USB Mini cable. The sensor is designed for general industry applications, test laboratories or smart home applications. The implementation of the sensor in the final application takes place by a virtual serial port and simple control commands.

## Data logger app in C#

We created an easy use data logger in C# to provide an easy and fast implementation of USB high temperature sensor in your target application.
<br><br>You can find the GitHub Repository here ➞ [GitHub Repository](https://github.com/BorisBloxsberg73/USB-high-temperature-sensor/tree/main)

## Driver

The sensor is using a virtual serial port or virtual comport (VCP). The driver of the virtual comport will be depolyed by the windows update service. If you are using another OS or your computer has no access to the internet, you can downlod the driver also directly from the FTDI website ➞ [FTDI driver download page](https://ftdichip.com/drivers/vcp-drivers/)

## Control commands

The control commands for the sensor are simple. The sensor has only two commands. Please find more details in the table below. Each command must end with an carriage return ("\r"). If you don´t send this sign, the sensor will not responding. The sensor also ends its response with a carriage return ("\r"). <br><br> (**Note**: The CRC calculation is descripted in the next section.)

<table border="2" bordercolor="#ff0000">
    <thead>
        <tr>
            <th width="20%">Command</th>
            <th width="40%">Command description</th>
            <th width="40%">Examble for response</th>     
        </tr>
    </thead>
    <tbody>
        <tr>
            <td align="center" valign="center">T?</td>
            <td align="left">Request of the current temperature of the sensor tip. The unit of the response is Celsius (°C) and won´t be transmitted.</td>
            <td align="left" valign="center">+545.4:2B&#10094;CR&#10095; (<b>Note:</b> &#10094;CR&#10095; means carrige return)</td>
        </tr>
        <tr>
            <td align="center" valign="center">I?</td>
            <td align="left">Request of the sensor information string. This response containing a lot of detailed static informations about the sensor e.g. Firmware version, output range. The response string is very long and needs some time to be transmitted from the sensor to the computer.</td>
            <td align="left" valign="center">Type:USB high temperature sensor&#10094;LF&#10095;Output unit:C&#10094;LF&#10095;...&#10094;CR&#10095;<br><br>(<b>Note:</b> &#10094;CR&#10095; means carrige return, &#10094;LF&#10095; means line feed) </td>
        </tr>
    </tbody>
</table>

### CRC 
The sensor response contains always an check sum. This check sum is used to prevent wrong data transmission or data manipulation. The CRC is separated by a double point from the pay load and is placed at the end of the sensor response. Below you will find further details and a calculation example.

#### CRC calculation algorithm
The CRC calculation algorithm sum up only the decimal values of the ASCII signs in front of the double point and adds this to the payload. It will be separated by a double point from the payload. 

In the following example the sensor transmitts a temperature of +545.4°C and the check sum 0x2B.
<table border="2" bordercolor="#ff0000">
    <thead>
        <tr>
            <th>Reponse from the sensor after T? command</th>
            <th width="7%">+</th>
            <th width="7%">5</th>
            <th width="7%">4</th>
            <th width="7%">5</th>
            <th width="7%">.</th>
            <th width="7%">4</th>
            <th width="7%">:</th>
            <th width="7%">2</th>
            <th width="7%">B</th>
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
           <br> <b>Note:</b> after 43+53+52+53+46+52 the 8 - bit var does overflow. So the Result turns to 44 and not to 299.</td>
        </tr>
    </tbody>
</table>

#### CRC calculation code example
```csharp
/// <summary>
/// this method calculates the CRC of the received data
/// </summary>
/// <param name="rawData">received  data string from the sensor</param>
/// <param name="crc">calculated CRC value of the raw data</param>
/// <returns>true -> CRC calculation succeed; false -> CRC calculation failed</returns>
public bool CalcCrc(string rawData, ref byte crc)
{
    //0 define vars
    bool rslt = false;

    //1 calculate CRC based on string
    //check rawData or sensor response regarding double point
    if (rawData.Contains(":"))
    {
        //#2 convert string to char array and find index of double point
        char[] charArray = rawData.ToCharArray();
        int posLastDoublePoint = rawData.LastIndexOf(":");

        //#2.1 sum up chars
        for (int i = 0; i < posLastDoublePoint; i++)
        {
            crc += (byte)charArray[i];
        }

        rslt = true;
    }

    //#3 return of the result
    return rslt; 
}
```
## Serial port setup

Please find below a C# code example for the serial port setup. 
**Note:** The name of the port depends on your system. Please double check your device manager for real portname because If you connect an other USB device the comport name could be changed.

```csharp
_SerialPort = new System.IO.Ports.SerialPort();
_SerialPort.PortName = "COM1"; //Note: "COM1" is just an examble
_SerialPort.BaudRate = 9600; //Note: Baud rate is defined by the sensor, do not change
_SerialPort.DataBits = 8;
_SerialPort.StopBits = System.IO.Ports.StopBits.One;
_SerialPort.NewLine = "\r"; //Note: the end sign is defined by the sensor, do not change 
_SerialPort.ReadTimeout = 500;
```

## FAQ and trouble shooting

- **What type of USB protocol does the sensor use?**
  - The sensor uses USB 2.0.

- **What type of USB connector does the sensor use?**
    - The sensor uses an USB Mini connector.

- **Does the sensor require an external power supply?**
    - No, the sensor is bus powered.

- **Is it possible to connect more than one sensor to my computer?**
    - Yes, it is. You can connect more than one sensor to a computer. Every sensor uses an own serial port.

- **Is there another app to communicate with the sensor?**
    - Yes, you could use every terminal program for the serial port e.g. HTerm or the HyperTerminal.
    - You could also code your own app.
  
- **The data logger app shows no comports.**
    - Please check the USB connection between the sensor and computer.
    - Change the USB port.
    - Use the USB ports at the backside of the computer.
    - Disconnect the sensor wait one minute, connect the sensor, wait again one minute and try again.
    - Check your device manager after unkown devices.

- **The sensor does not respond after the descripted commands.**
    - Please check your command. 
    - Please only use capital letters for the command string and donn´t insert space in the command.
    - Check your command regarding the carriage return. The senosr needs the carriage return to find the command end.
    - Check the baud rate of the serial port. The required baud rate is 9600 kbits.

- **The sensor transmitts only cryptic signs.**
    - Check the Baudrate of the serial port. The required baud rate is 9600 kbits.

- **The sensor transmitts only temperatures above 1200°C.**
    - The sensor is maybe damaged and transmitting an diagnosis code.


