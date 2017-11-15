using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;
using System.IO;

namespace ASPNetDemoWebApplication
{
    public class Logger
    {

        public static void Log(Exception exception)
        {
            Log(exception, EventLogEntryType.Error);
        }
        public static void Log(Exception exception, EventLogEntryType eventLogEntryType)
        {
            var sbExceptionMessage = new StringBuilder();
            do
            {
                sbExceptionMessage.Append("Exception Type: " + Environment.NewLine);
                sbExceptionMessage.Append(exception.GetType().Name + Environment.NewLine);
                sbExceptionMessage.Append("/n" + Environment.NewLine);
                sbExceptionMessage.Append("Message: " + Environment.NewLine);
                sbExceptionMessage.Append(exception.Message + Environment.NewLine);
                sbExceptionMessage.Append(Environment.NewLine + Environment.NewLine);
                sbExceptionMessage.Append("Stack Trace: " + Environment.NewLine);
                sbExceptionMessage.Append(exception.StackTrace + Environment.NewLine);

                exception = exception.InnerException;
            } while (exception != null);

            string logProvider = ConfigurationManager.AppSettings["LogProvider"];
            if (logProvider.ToLower() == "database")
            {
                LogToDB(sbExceptionMessage.ToString());
            }
            else if (logProvider.ToLower() == "eventviewer")
            {
                LogToEventViewer(sbExceptionMessage.ToString(), eventLogEntryType);
            }
            else if (logProvider.ToLower() == "SendEmail")
            {
                SendLogToEmail(sbExceptionMessage.ToString();
            }
            else
            {
                LogToDB(sbExceptionMessage.ToString());
                LogToEventViewer(sbExceptionMessage.ToString(), eventLogEntryType);
                SendLogToEmail(sbExceptionMessage.ToString());
            }
        }

        private static void LogToEventViewer(string sbExceptionMessage, EventLogEntryType eventLogEntryType)
        {
            if (!EventLog.SourceExists("ApplicationLogs.com"))
            {
                // if the source is not there in the event viewer then create one
                EventLog.CreateEventSource("ApplicationLogs.com", "ApplicationLogs");
            }
            else
            {
                EventLog log = new EventLog("ApplicationLogs");
                log.Source = "ApplicationLogs.com";

                log.WriteEntry(sbExceptionMessage.ToString(), eventLogEntryType);
            }
        }
        private static void LogToDB(String sbExceptionMessage)
        {
            string conString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand("spInsertLog", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            SqlParameter param = new SqlParameter("@ExceptionMessage", sbExceptionMessage.ToString());
            cmd.Parameters.Add(param);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        private static void SendLogToEmail(string emailBody)
        {
            System.Net.Mail.MailMessage mailMessage = new MailMessage("venk120soft@gmail.com", "venk120soft@gmail.com");
            mailMessage.Subject = "Exception";
            mailMessage.Body = emailBody;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);
        }
        private static void LogToTextFile(string sbExceptionMessage)
        {
            //Using System.IO
            string filepath = "c:/somefile.txt";//context.current.server
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            filepath = filepath + DateTime.Today.ToString() + ".txt";
            if (File.Exists(filepath))
            {
                File.Create(filepath).Dispose();
            }
            using (StreamWriter sw = File.AppendText(filepath))
            {
                sw.WriteLine("Log Wriiten date --Details On-- " + DateTime.Now.ToString());
                sw.WriteLine(sbExceptionMessage);
                sw.Flush();
                sw.Close();
            }
        }
    }
}