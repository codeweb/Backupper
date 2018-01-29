using Backupper.Code;
using log4net;
using System.Linq;

namespace Backupper
{
    class Program
    {
        public static void Main(string[] args)
        {
            ILog logger = LogManager.GetLogger("Backupper");
            BackupManager bm = new BackupManager(logger);
            bm.AvviaElaborazione();            
            //Console.ReadLine();
        }

        
    }
}
