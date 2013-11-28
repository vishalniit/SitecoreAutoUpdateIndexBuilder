using log4net;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoUpdateIndexer
{
    public static class MyIndex
    {
        public static ILog log = log4net.LogManager.GetLogger(typeof(MyIndex));
        public static string[] GetAllIndexDirectory()
        {
            string[] ii = null;
            System.IO.DirectoryInfo dif = new System.IO.DirectoryInfo(UtilitySettings.OuterIndexFolder);
            if (dif.Exists)
            {
                System.IO.DirectoryInfo[] difs = dif.GetDirectories();
                ii = new string[difs.Length];
                int i = 0;
                foreach (System.IO.DirectoryInfo di in difs)
                {
                    if (di.Exists)
                    {
                        if (di.Name.ToLower().Contains(UtilitySettings.ProductIndexName.ToLower()))
                        {
                            ii[i] = di.FullName;
                            i++;
                        }
                        else if (di.Name.ToLower().Contains(UtilitySettings.ContentIndexName.ToLower()))
                        {
                            ii[i] = di.FullName;
                            i++;
                        }
                        else if (di.Name.ToLower().Contains(UtilitySettings.FileIndexName.ToLower()))
                        {
                            ii[i] = di.FullName;
                            i++;
                        }
                        else if (di.Name.ToLower().Contains(UtilitySettings.GlobalReferenceIndexName.ToLower()))
                        {
                            ii[i] = di.FullName;
                            i++;
                        }
                    }
                }
            }
            else
            {                
                log.Error(string.Format("Given index Directory {0} Doesn't Exist", UtilitySettings.OuterIndexFolder));
            }

            return ii;
        }

        public static FSDirectory GetAutoUpdateIndexDirectory()
        {
            string autoUpdate = UtilitySettings.OuterIndexFolder + @"\" + UtilitySettings.AutoUpdateIndexName;
            try
            {
                if (!System.IO.Directory.Exists(autoUpdate))
                    System.IO.Directory.CreateDirectory(autoUpdate);
            }
            catch (Exception ex)
            {
                log.Error("Error Occurred during GetAutoUpdateIndexDirectory()", ex);
            }
            FSDirectory fsd = null;
            if (System.IO.Directory.Exists(autoUpdate))
            {
                fsd = FSDirectory.Open(new System.IO.DirectoryInfo(autoUpdate));
            }
            return fsd;
        }
    }
}
