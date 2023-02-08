
namespace USB_data_logger.Models
{
    /// <summary>
    /// this object contains information about the machine and the app
    /// </summary>
    public class DataInformationOs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="osName">Name of the operating system</param>
        /// <param name="userName">Windows user name</param>
        /// <param name="machineName">Name of the machine/computer</param>
        /// <param name="appVersion">Version of the application</param>
        public DataInformationOs(string osName, string userName, string machineName, string appVersion)
        {
            OsName = osName;
            UserName = userName;
            MachineName = machineName;
            AppVersion = appVersion;
        }  
        
        /// <summary>
        /// Name of the operating system
        /// </summary>
        public readonly string OsName;

        /// <summary>
        /// Windows user name
        /// </summary>
        public readonly string UserName;

        /// <summary>
        /// Name of the machine/computer
        /// </summary>
        public readonly string MachineName;

        /// <summary>
        /// Version of the application
        /// </summary>
        public readonly string AppVersion;
    }
}
