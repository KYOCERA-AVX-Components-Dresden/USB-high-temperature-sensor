using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using USB_data_logger.Models;
using USB_data_logger.Pheripherals;
using Timer = System.Timers.Timer;

namespace USB_data_logger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //init UI and controls
            InitializeComponent();

            //init UI and fill this with data
            InitApp();
        }
        
        #region Member
        private DataInformationOs _DataInformationOs;
            private DataInformationSensor? _DataInformationSensor;
            private Timer _Timer;
            private UsbHighTemperatureSensor? _UsbSensor;
        #endregion

        #region Methods

        /// <summary>
        /// this method initialize the app and the UI
        /// </summary>
        private void InitApp()
        {
            //#0 define vars
            string osInfo = "";

            //#1 read and add App version to UI
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            LblVersion.Content = assembly.GetName().Version;
            
            //#3 Create general OS information
            _DataInformationOs = new DataInformationOs(
                "Windows " + Environment.OSVersion.VersionString,
                Environment.UserName,
                Environment.MachineName,
                assembly.GetName().Version.ToString());

            //#3.1 display OS information
            osInfo = "OS: " + _DataInformationOs.OsName + "\n"
                     + "Username: " + _DataInformationOs.UserName + "\n"
                     + "Machine name: " + _DataInformationOs.MachineName + "\n"
                     + "App version: " + _DataInformationOs.AppVersion ;
            LblOsInformationData.Content = osInfo;
        }

        /// <summary>
        /// this method initialize the data acquisition timer
        /// </summary>
        private void InitTimer()
        { 
            //#0 destroy old timer object
            if(_Timer != null)
            {
                _Timer.Stop();
                _Timer.Dispose();  
            }
                 
            //#1 config timer 
            _Timer = new Timer(500);
            _Timer.Elapsed += TimDataAcq_Tick;
        }

        /// <summary>
        /// this method initialize the sensor and the virtual comport
        /// </summary>
        private void InitSensor()
        {
            //#0 define vars
            string comportName = "";

            //#1 check selection of combo box
            if (CboComport.SelectedIndex >= 0)
            {
                String errorMessage = "";
                comportName = CboComport.Text;

                //#2 initialize timer
                InitTimer();

                //#3 initialise usb device
                if (_UsbSensor != null) _UsbSensor.Dispose();
                _UsbSensor = new UsbHighTemperatureSensor();
                if (_UsbSensor.Init(comportName, ref errorMessage))
                {
                    //#3.1 read general information
                    _UsbSensor.GetSensorInformation(ref _DataInformationSensor);

                    //#3.2 display information
                    LblSensorInformationData.Content = _DataInformationSensor.RawData;
                    
                    //#4 Start data acquisition
                    _Timer.Start();
                }
                else
                {
                    MessageBox.Show("Can not initialize application. \n\nError: " + errorMessage, "", MessageBoxButton.OK);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please select a comport.", "", MessageBoxButton.OK);
                return;
            }       
        }
        #endregion

        #region Events

            private void BtnClose_Click(object sender, RoutedEventArgs e)
            {
                //close app
                string msg = "Are you sure?";
                if (MessageBox.Show(msg, "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if(_Timer!=null) _Timer.Stop(); 
                    Close();
                }
            }

            private void TimDataAcq_Tick(Object source, ElapsedEventArgs e)
            {
                //#0 define vars
                DataMeasurementSensor? dataMeasurementSensor = null;
                string outputValue = "";
                string date = "";

                //#1 read sensor data
                if (_UsbSensor.GetSensorOutput(ref dataMeasurementSensor))
                { 
                    outputValue = dataMeasurementSensor.Output.ToString("F1") + " " + _DataInformationSensor.Unit;
                    date = dataMeasurementSensor.OutputTimeStamp.ToString("dd/MM/yyyy hh:mm:ss.fff");
                }
                else
                {
                    date = "Can not read any data.";
                }

                //#2 display sensor data
                this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => LblSensorOutputCurrentValue.Content = outputValue));
                this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => LblSensorOutputTimeStamp.Content = date));
            }

            private void BtnInit_Click(object sender, RoutedEventArgs e)
            {
                InitSensor();        
            }

            private void Grid_MouseMove(object sender, MouseEventArgs e)
            {
                //move app
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }

            private void CboComport_PreviewMouseDown(object sender, MouseButtonEventArgs e)
            {
                //#1 delete old items in combobox
                CboComport.Items.Clear();

                //#2 add comports to UI
                foreach (var item in System.IO.Ports.SerialPort.GetPortNames())
                {
                    CboComport.Items.Add(item);
                }
            }
            #endregion
    }
}
