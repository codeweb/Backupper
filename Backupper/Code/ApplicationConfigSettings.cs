using System;
using System.Data;
using System.Configuration;



namespace Base.Configuration
{
    /// <summary>
    /// Espone tutti le impostazioni di configurazione presenti nella sezione 
    /// appSettings di web.config. 
    /// </summary>
    public static class ApplicationConfigSettings
    {

        public static string MailUtenteDaAvvisareInCasoDiErrore
        {
            get { return WebConfig.GetString("MailUtenteDaAvvisareInCasoDiErrore"); }
        }

        public static string CartellaDestinazione
        {
            get { return WebConfig.GetString("CartellaDestinazione"); }
        }

        public static string PatternFile
        {
            get { return WebConfig.GetString("PatternFile"); }
        }

        public static int NumeroGiorni
        {
            get { return WebConfig.GetInt32("NumeroGiorni"); }
        }

        public static string SmtpServer
        {
            get { return WebConfig.GetString("SmtpServer"); }
        }

        public static string SmtpUsername
        {
            get { return WebConfig.GetString("SmtpUsername"); }
        }

        public static string SmtpPassword
        {
            get { return WebConfig.GetString("SmtpPassword"); }
        }

        public static string SmtpSenderAddress
        {
            get { return WebConfig.GetString("SmtpSenderAddress"); }
        }

        public static string ServerNamePerLog
        {
            get { return WebConfig.GetString("ServerNamePerLog"); }
        }

        public static string Databases
        {
            get { return WebConfig.GetString("Databases"); }
        }

        public static string SevenZipDllPath
        {
            get { return WebConfig.GetString("SevenZipDllPath"); }
        }

        public static int CommandTimeout
        {
            get { return WebConfig.GetInt32("CommandTimeout"); }
        }       

        public static int SmtpPort
        {
            get { return WebConfig.GetInt32("SmtpPort"); }
        }

        public static bool SmtpEnableSsl
        {
            get { return WebConfig.GetBool("SmtpEnableSsl"); }
        }

        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString; }
        }
    }
}
