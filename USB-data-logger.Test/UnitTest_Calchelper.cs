using USB_data_logger.Calculations;
using USB_data_logger.Models;

namespace USB_data_logger.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("+1227.0:55", 85,Description = "Test ID 1.1: Good case")]
        [TestCase("-23.0:F0", 240, Description = "Test ID 1.2: Good case")]
        [TestCase("+22AA", 0, Description = "Test ID 1.3: bad case, no double point")]
        [TestCase("", 0, Description = "Test ID 1.4: Bad case, no data")]
        public void GetCrc_Test(string rawData, int crcExpected)
        {
            //#0 define vars
            var calcHelper = new CalcHelper_UsbSensor();

            //#1 execute test
            var crcCalculated  = calcHelper.GetCrc(rawData);
            Console.WriteLine($"Expected result: {crcExpected}, Calculated result: {crcCalculated}");

            //2 verify result
            Assert.IsTrue(crcCalculated==crcExpected);
        }

        [TestCase("+1227.0:55", true, Description = "Test ID 2.1: Good case")]
        [TestCase("-23.0:F0", true, Description = "Test ID 2.2: Good case")]
        [TestCase("+22AA", false, Description = "Test ID 2.3: bad case, no double point")]
        [TestCase("", false, Description = "Test ID 2.4: Bad case, no data")]
        public void CheckCrc_Test(string rawData, bool rsltExpected)
        {
            //#0 define vars
            var calcHelper = new CalcHelper_UsbSensor();

            //#1 execute test
            var rsltCalculated = calcHelper.CheckCrc(rawData);
            Console.WriteLine($"Expected result: {rsltExpected}, Calculated result: {rsltCalculated}");

            //2 verify result
            Assert.IsTrue(rsltCalculated == rsltExpected,  $"Expected result: {rsltExpected}, Calculated result: {rsltCalculated}");
        }
             
        [TestCase("+1227.0:55", +1227.0d,true, Description = "Test ID 3.1: Good case")]
        [TestCase("-23.0:F0", -23d, true, Description = "Test ID 3.2: Good case")]
        [TestCase("+22AA", 0,false, Description = "Test ID 3.3: bad case, no double point")]
        [TestCase("", 0, false, Description = "Test ID 3.4: Bad case, no data")]
        [TestCase("+727.0:29", +727.0d, true, Description = "Test ID 3.5: Good case")]
        [TestCase("+72.5:F7", +72.5d, true, Description = "Test ID 3.6: Good case")]
        [TestCase("+227.0:24", 227.0d, true, Description = "Test ID 3.7: Good case")]
        public void ConvertSensorOutput_Test(string rawData, double outputValueExpected,bool rsltExpectedConversation )
        {
            //#0 define vars
            var calcHelper = new CalcHelper_UsbSensor();
            DataMeasurementSensor dataMeasurement = null;

            //#1 execute test
            var rsltConversation = calcHelper.ConvertSensorOutput(rawData, ref dataMeasurement);
         
            //2 verify result
            if (rsltConversation)
            {  
                Console.WriteLine($"Expected result: {outputValueExpected}, Calculated result: {dataMeasurement.Output}");
                Assert.IsTrue(dataMeasurement.Output == outputValueExpected,
                    $"Expected result: {outputValueExpected}, Calculated result: {dataMeasurement.Output}");
            }
            else
            {
                Assert.IsFalse(rsltConversation);
            }          
        }

        [Test]
        public void ConvertSensorInformation_Test()
        {
            //#0 define vars
            CalcHelper_UsbSensor calcHelper = new CalcHelper_UsbSensor();
            DataInformationSensor dataInformationSensor = null;

            var testData = "Type:USB high temperature sensor\nOutput unit:C\nOutput range min:-40.0\n" +
                           "Output range max:+1200.0\nCommand:I?, General sensor information\nCommand:T?, Temperature sensor tip in Celcius" +
                           "\nSoftwarever:09\nHardwarever:03\n:0B";

            //#1 execute test
            var rsltConversation = calcHelper.ConvertSensorInformation(testData, ref dataInformationSensor);

            var rsltData = dataInformationSensor.FirmwareVersion == 9;
            rsltData &= dataInformationSensor.HardwareVersion == 3;
            rsltData &= dataInformationSensor.Type == "USB high temperature sensor";
            rsltData &= dataInformationSensor.OutputMax == 1200d;
            rsltData &= dataInformationSensor.OutputMin == -40d;
            rsltData &= dataInformationSensor.Unit == "C";

            //2 verify result
            Assert.IsTrue(rsltConversation && rsltData);
            
        }
    }
}