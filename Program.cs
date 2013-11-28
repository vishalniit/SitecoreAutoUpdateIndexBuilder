using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System.Text;
using System.Diagnostics;
using System.Linq;
using log4net;

namespace AutoUpdateIndexer
{
    internal class Program
    {
        public static ILog log;

        private static string Showhelp()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Please run this application as follows :");
            sb.Append(System.Environment.NewLine);
            sb.Append("*.exe [M] [Term]");
            sb.Append(System.Environment.NewLine);
            sb.Append("Here *.exe indicate the application executable name");
            sb.Append(System.Environment.NewLine);
            sb.Append("[M] means mode which has following valid argument:- ");
            sb.Append(System.Environment.NewLine);
            sb.Append(" 'B' it will build the index simply");
            sb.Append(System.Environment.NewLine);
            sb.Append(" 'BV' it will also build the index but in Verbose Mode, means it will list out each word getting indexed runtime");
            sb.Append(System.Environment.NewLine);
            sb.Append(" 'S' it will allow you to search on the build index for given term, this option is used for the debugging. ");
            sb.Append(System.Environment.NewLine);
            sb.Append("When you give this option you also need to specify [Term] after space of first parameter");
            sb.Append(System.Environment.NewLine);
            sb.Append("[Term] should greater then 1 in length of characters");
            sb.Append(System.Environment.NewLine);
            sb.Append("Valid Examples:- ");
            sb.Append(System.Environment.NewLine);
            sb.Append("M1.AutoUpdateIndexer.exe B");
            sb.Append(System.Environment.NewLine);
            sb.Append("M1.AutoUpdateIndexer.exe BV");
            sb.Append(System.Environment.NewLine);
            sb.Append("M1.AutoUpdateIndexer.exe S So");
            sb.Append(System.Environment.NewLine);
            sb.Append("In Last example user is trying to search term starting with 'so' which could be sony sonar etc.,:- ");
            sb.Append(System.Environment.NewLine);
            return sb.ToString();
        }

        private static void Main(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();
            log = log4net.LogManager.GetLogger(typeof(Program));
            log.Info(System.Environment.NewLine);
            log.Info("------------------------------Start--------------------------------------------");
            log.Info(System.Environment.NewLine);
            log.Info(string.Format("Entered in Main Program at time {0}", DateTime.Now.ToString()));
            if (args == null && args.Length < 1 && args.Length > 2)
            {
                log.Info(Showhelp());
            }
            else if (args[0] == null || args[0].Length <= 0)
            {
                log.Info(Showhelp());
            }
            else
            {
                switch (args[0])
                {
                    case "B":
                        buildIndex(false);
                        break;
                    case "BV":
                        buildIndex(true);
                        break;
                    case "S":
                        if (args[1] != null && args[1].Length > 2)
                        {
                            string[] suggestions = searchTerm(args[1]);
                            if (suggestions != null && suggestions.Length > 0)
                            {
                                foreach (string suggest in suggestions)
                                {
                                    log.Info(suggest);
                                }
                            }
                            else
                            {
                                log.Info("Your Search Term Doesn't fetch any result");
                            }
                        }
                        else
                        {
                            log.Info(Showhelp());
                        }
                        break;
                    default:
                        log.Info(Showhelp());
                        break;
                }
            }
            log.Info(System.Environment.NewLine);
            log.Info("--------------------------------Finished------------------------------------------");
            log.Info(System.Environment.NewLine);
            log.Info(System.Environment.NewLine);
        }

        private static void buildIndex(bool buildVerbose)
        {
            try
            {
                string[] si = MyIndex.GetAllIndexDirectory();
                Stopwatch sw = new Stopwatch();
                CleanIndexIfApplicable();
                SearchAutoComplete searchIndex = null;
                bool flagFirstTime = true;
                if (si != null && si.Length > 0)
                {
                    foreach (string directory in si)
                    {
                        if (directory != null)
                        {
                            searchIndex = new SearchAutoComplete(directory);
                            searchIndex.IsFirstTime = flagFirstTime;
                            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(directory);
                            if (di.Exists)
                            {
                                log.Info(string.Format("Index in directory {0} is building now...", di.Name));
                                FSDirectory fsd = FSDirectory.Open(di);
                                Lucene.Net.Index.CheckIndex ci = new CheckIndex(fsd);
                                Lucene.Net.Index.CheckIndex.Status st = ci.CheckIndex_Renamed_Method();
                                if (st.clean)
                                {
                                    sw.Start();
                                    searchIndex.BuildAutoCompleteIndex(fsd, MyIndex.GetAutoUpdateIndexDirectory(), buildVerbose);
                                    sw.Stop();
                                    TimeSpan ts = sw.Elapsed;
                                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                                    log.Info(string.Format("Time taken {0} to build this index {1}", elapsedTime, di.Name));
                                    flagFirstTime = false;
                                }
                                else
                                {
                                    log.Info(string.Format("Following Index named {0} is corrupted, please rebuild that index using Sitecore Rebuild Index Wizard", di.Name));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error Occurred during buildIndex().", ex);
            }
        }

        private static string[] searchTerm(string term)
        {
            string[] suggestions = null;

            if (term != null && term.Length > 1)
            {
                try
                {
                    SearchAutoComplete searchIndex1 = new SearchAutoComplete(MyIndex.GetAutoUpdateIndexDirectory());
                    suggestions = searchIndex1.SuggestTermsFor(term);
                    if (suggestions != null && suggestions.Length > 1)
                        suggestions = suggestions.Distinct().ToArray<string>();
                }
                catch (Exception ex)
                {
                    log.Error("Error Occurred during searchTerm() and term is " + term + ". ", ex);
                }
            }
            return suggestions;
        }

        private static bool CleanIndexIfApplicable()
        {
            bool result = false;
            try
            {
                FSDirectory AutoUpdateIndexDirectory = MyIndex.GetAutoUpdateIndexDirectory();
                if (AutoUpdateIndexDirectory != null)
                {
                    DateTime dtLastModifiedTime = AutoUpdateIndexDirectory.GetDirectory().LastWriteTime;
                    DateTime CurrentDateTime = DateTime.Now;
                    double days = (CurrentDateTime - dtLastModifiedTime).TotalDays;
                    if (days > 7)
                    {
                        log.Info("Inside CleanIndexIfApplicable");
                        log.Info(string.Format("Last Modified Date {0}", dtLastModifiedTime));

                        log.Info("Going to Delete old index files");

                        DirectoryInfo dif = AutoUpdateIndexDirectory.GetDirectory();
                        foreach (FileInfo file in dif.GetFiles())
                        {
                            try
                            {
                                log.Info(string.Format("Deleting {0}", file.Name));
                                file.Delete();
                            }
                            catch
                            {
                            }
                        }
                        dif.Refresh();
                        if (dif.GetFiles().Count() == 0)
                        {

                            log.Info("Cleanup Completed");
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error Occurred during CleanIndexIfApplicable()", ex);
            }
            return result;
        }
    }
}