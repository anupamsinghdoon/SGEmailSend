using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendGridEmailSend
{
    class Logger
    {
        static string GetEmailsProc = ConfigurationManager.AppSettings["GetEmailsProc"].ToString();
        #region Log It
        public static void LogIt(System.Reflection.MethodBase methodBase, string Message)
        {
            try
            {
                if (methodBase == null) methodBase = System.Reflection.MethodBase.GetCurrentMethod();
                string fullMethodName = methodBase.DeclaringType.FullName + "." + methodBase.Name + "()";                
                String LogPath = string.Format("{0}{1}-{2}-{3}.log", GetLogFolder(), GetLoginName(), DateTime.Now.ToString("yyyyMMdd"), GetEmailsProc);
                Message = string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}\t{1}\t{2}", DateTime.Now.ToString(), fullMethodName, Message) + Environment.NewLine;
                System.IO.File.AppendAllText(LogPath, Message);
            }
            catch (Exception ex)
            {
                Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), "Error: " + ex.Message.ToString());
            }
        }
        public static void ErrorLogIt(System.Reflection.MethodBase methodBase, string Message)
        {
            try
            {
                if (methodBase == null) methodBase = System.Reflection.MethodBase.GetCurrentMethod();
                string fullMethodName = methodBase.DeclaringType.FullName + "." + methodBase.Name + "()";
                String LogPath = string.Format("{0}Error-{1}-{2}-{3}.log", GetLogFolder(), GetLoginName(), DateTime.Now.ToString("yyyyMMdd"), GetEmailsProc);
                Message = string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}\t{1}\t{2}", DateTime.Now.ToString(), fullMethodName, Message) + Environment.NewLine;
                System.IO.File.AppendAllText(LogPath, Message);
            }
            catch (Exception ex)
            {
                Logger.ErrorLogIt(System.Reflection.MethodBase.GetCurrentMethod(), "Error: " + ex.Message.ToString());
            }
        }
        public static void LogLimit(string strLogPath)
        {
            try
            {
                string[] strFiles = Directory.GetFiles(strLogPath);

                for (int i = 0; i < strFiles.Length; i++)
                {
                    FileInfo myFileInfo = new FileInfo(strFiles[i]);
                    DateTime fileTime = myFileInfo.CreationTime;
                    if (fileTime < DateTime.Now.AddDays(-30))
                    {
                        File.Delete(strFiles[i]);
                        Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), "Log file : " + Path.GetFileName(strFiles[i]) + " deleted.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), "Error : " + ex.Message.ToString());
            }
        }

        private static string LogFolder = null;
        public static string GetLogFolder()
        {
            if (LogFolder == null)
            {
                try
                {
                    string currentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    if (!Directory.Exists(currentPath + "\\" + "Logs"))
                        Directory.CreateDirectory(currentPath + "\\" + "Logs");
                    LogFolder = currentPath + @"\Logs\";
                }
                catch (Exception ex)
                {

                    Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), "Error: " + ex.Message.ToString());
                }

                if (LogFolder.StartsWith(@"\\"))
                {
                    //this is a network drive, use the share
                    string[] pathParts = LogFolder.Split('\\');
                    for (int i = 0; i < pathParts.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(pathParts[i]))
                        {
                            LogFolder = string.Format(@"\\{0}\Logs\", pathParts[i]);
                            break;
                        }
                    }
                }
            }
            return LogFolder;
        }

        private static string LoginName = null;
        public static string GetLoginName()
        {
            if (LoginName == null)
            {
                // cache it for best performance
                string[] userNameParts = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split(@"\".ToCharArray());
                LoginName = userNameParts[userNameParts.Length - 1];
            }
            return LoginName;
        }
        #endregion
    }
}
