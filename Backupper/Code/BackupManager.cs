using log4net;
using System;
using System.Data.SqlClient;
using SevenZip;
using Base.Configuration;
using System.Linq;
using System.IO;

namespace Backupper.Code
{
    public class BackupManager
    {
        private ILog Logger { get; set; }
        private string Testo { get; set; }
        private string Oggetto { get; set; }

        public BackupManager(ILog logger)
        {
            Logger = logger;
            Testo = ApplicationConfigSettings.ServerNamePerLog + ": Questa mail è stata inviata automaticamente da Backupper il " + DateTime.Now;
            Oggetto = ApplicationConfigSettings.ServerNamePerLog + ": backup database ";
        }

        private bool DoBackup()
        {
            string[] databases = ApplicationConfigSettings.Databases.Split(',');

            var sqlConStrBuilder = new SqlConnectionStringBuilder(ApplicationConfigSettings.ConnectionString);
            bool esitoOperazione = true;
            foreach (string database in databases)
            {
                try
                {
                    LogText(String.Format("{0}: Inizio Backup database...", database));
                    string destinationFilename = String.Format("{0:yyyyMMdd}_{1}", DateTime.Now, database);
                    string destinationFilenameBak = String.Format("{0}.bak", destinationFilename);
                    string destination = String.Format("{0}\\{1}", ApplicationConfigSettings.CartellaDestinazione, destinationFilenameBak);
                    using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                    {
                        var query = String.Format("BACKUP DATABASE [{0}] TO DISK='{1}' WITH INIT;",
                            database, destination);

                        using (var command = new SqlCommand(query, connection))
                        {
                            connection.Open();
                            command.CommandTimeout = ApplicationConfigSettings.CommandTimeout;
                            command.ExecuteNonQuery();
                        }

                    }
                    LogText(String.Format("{0}: Fine Backup database", database));
                    bool esito = DoZipFiles(destinationFilename, destination);
                    bool esitoRemove = true;
                    if (esito)
                    {
                        // rimuovi file bak
                        esitoRemove = RemoveFileBak(destination);
                    }
                    esitoOperazione = esitoOperazione && esito && esitoRemove;
                }
                catch (Exception ex)
                {
                    string errore = String.Format("{0}: Errore backup database", database);
                    LogText(errore, LogLevel.Error, ex);
                    return false;
                }
            }
            return esitoOperazione;
        }

        private bool RemoveFileBak(string pathFile)
        {
            try
            {
                LogText(String.Format("{0}: Inizio rimozione file bak...", pathFile));
                FileInfo fileBak = new FileInfo(pathFile);
                if (fileBak.Exists)
                {
                    fileBak.Delete();
                    LogText(String.Format("{0}: Eliminato file bak ", pathFile));
                }
                return true;
            }
            catch (Exception ex)
            {
                string errore = String.Format("{0}: Errore rimozione file bak", pathFile);
                LogText(errore, LogLevel.Error, ex);
                return false;
            }
        }

        private bool DoZipFiles(string destinationFilename, string fileBak)
        {
            try
            {
                string destinationFileZip = String.Format("{0}.zip", destinationFilename);
                string destination = String.Format("{0}\\{1}", ApplicationConfigSettings.CartellaDestinazione, destinationFileZip);
                using (Stream outStream = new FileStream(destination, FileMode.Create))
                {
                    LogText(String.Format("{0}: Inizio creazione file zip...", destination));
                    SevenZipCompressor.SetLibraryPath(ApplicationConfigSettings.SevenZipDllPath);
                    SevenZipCompressor szc = new SevenZipCompressor
                    {
                        ArchiveFormat = OutArchiveFormat.Zip,
                        CompressionLevel = CompressionLevel.Ultra,
                        CompressionMode = CompressionMode.Create
                    };

                    szc.CompressFiles(outStream, fileBak);
                }
                LogText(String.Format("{0}: Creato file zip", destination));
                return true;
            }
            catch (Exception ex)
            {
                string errore = String.Format("{0}: Errore zip file", destinationFilename);
                LogText(errore, LogLevel.Error, ex);
                return false;
            }
        }

        private void EliminaFileZip(string cartella)
        {
            DirectoryInfo d = new DirectoryInfo(cartella);
            FileInfo[] fileZipDaCancellare = d.GetFiles(ApplicationConfigSettings.PatternFile);

            LogText(String.Format("{0}: Presenti {1} file del tipo specificato [{2}] ", cartella, fileZipDaCancellare.Length, ApplicationConfigSettings.PatternFile));
            int contatore = 0;
            foreach (FileInfo f in fileZipDaCancellare)
            {
                try
                {
                    int result = 0;
                    if (f.Name.Length > 8 && Int32.TryParse(f.Name.Substring(0, 8), out result))
                    {
                        // controllo contenga la data
                        DateTime dataFile = new DateTime(Convert.ToInt32(f.Name.Substring(0, 4)), Convert.ToInt32(f.Name.Substring(4, 2)), Convert.ToInt32(f.Name.Substring(6, 2)));
                        if (dataFile <= DateTime.Now.AddDays(-ApplicationConfigSettings.NumeroGiorni))
                        {
                            f.Delete();
                            contatore += 1;
                            LogText(String.Format("{0}: Eliminato file ", f.FullName));
                        }
                    }
                    else
                    {
                        LogText(String.Format("{0}: File {1}: length <= 8 oppure {2} not int", cartella, f.FullName, f.Name.Substring(0, 8)), LogLevel.Warning);
                    }
                }
                catch (Exception ex)
                {
                    string errore = String.Format("{0}: Errore sul file {1}", cartella, f.FullName);
                    LogText(errore, LogLevel.Error, ex);
                }
            }

            LogText(String.Format("{0}: Eliminati {1} file zip", cartella, contatore));
        }

        private void LogText(string input, LogLevel logLevel = LogLevel.Info, Exception ex = null)
        {
            switch (logLevel)
            {
                case LogLevel.Info:
                default:
                    Logger.Info(input);
                    break;
                case LogLevel.Warning:
                    Logger.Warn(input);
                    break;
                case LogLevel.Error:
                    if (ex != null)
                        Logger.Error(input, ex);
                    else
                        Logger.Error(input);
                    break;
            }
            Testo += String.Format("<br/>{0}", input);
            if (ex != null)
            {
                Testo += String.Format("<br/>{0}", ex.ToString());
            }
        }

        public void AvviaElaborazione()
        {
            try
            {
                LogText("Inizio elaborazione...");

                string cartella = ApplicationConfigSettings.CartellaDestinazione;
                if (String.IsNullOrEmpty(cartella.Trim()))
                    LogText("Non ci sono cartelle da elaborare", LogLevel.Warning);
                else
                if (String.IsNullOrEmpty(ApplicationConfigSettings.Databases))
                    LogText("Non ci sono database da elaborare", LogLevel.Warning);
                else
               if (!Directory.Exists(cartella))
                    LogText("La cartella da elaborare non esiste", LogLevel.Warning);
                else
                {
                    bool esitoOk = DoBackup();
                    if (!esitoOk)
                    {
                        throw new Exception("Si sono verificati degli errori nell'elaborazione");
                    }

                    // eliminazione file zip vecchi
                    EliminaFileZip(cartella);
                }
                
            }
            catch (Exception ex)
            {
                LogText("Errore elaborazione", LogLevel.Error, ex);
                Oggetto = ApplicationConfigSettings.ServerNamePerLog + ": Errore in backup database ";
            }
            finally
            {
                LogText("...Fine elaborazione");
                LogText("-------------------------------");
                LogText("-------------------------------");
            }

            try
            {
                EmailUtil.Send(Logger, ApplicationConfigSettings.SmtpServer, ApplicationConfigSettings.SmtpUsername, ApplicationConfigSettings.SmtpPassword, ApplicationConfigSettings.SmtpSenderAddress, ApplicationConfigSettings.SmtpSenderAddress, ApplicationConfigSettings.MailUtenteDaAvvisareInCasoDiErrore, Oggetto, Testo, true, string.Empty, null, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                LogText("Errore invio email", LogLevel.Error, ex);
            }
            
        }
    }
}
