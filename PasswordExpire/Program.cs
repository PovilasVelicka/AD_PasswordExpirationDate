
using BaltnetaSmsApi;
using System;
using System.Configuration;
using System.IO;

namespace PasswordExpire
{

    internal class Program
    {
        const string LOG_FILE_NAME = "password_expiration.log";
        const string REPORT_FILE_NAME = "users_report.csv";

        private static readonly string _logFile;
        private static readonly string _reportFile;

        private static readonly string[ ] _groups;
        private static readonly SmsService _smsService;
        private static readonly string _smsText;

        static Program ( )
        {
            var appLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly( ).Location);

            _logFile = appLocation + Path.DirectorySeparatorChar + LOG_FILE_NAME;     

            _reportFile = appLocation+ Path.DirectorySeparatorChar + REPORT_FILE_NAME;

            if (!File.Exists(_reportFile))
            {
                AppendLineToFile("Report date\tUser name\tMobile number\tLast password set date\tEnabled\tSend sms", _reportFile);
            }

            _smsText = ConfigurationManager.AppSettings["smsMessage"];
            _smsService = new SmsService(
                apiKey: ConfigurationManager.AppSettings["apiKey"],
                login: ConfigurationManager.AppSettings["login"]);

            _groups = ConfigurationManager.AppSettings["Groups"].Split(',');

        }

        static void Main (string[ ] args)
        {
            var adUserService = new AdService( );

            UserDelegate sendMethods = PrintConsoleMessage;
            sendMethods += SendSmsMessage;
            sendMethods += SaveToReport;
            foreach (var group in _groups)
            {
                Console.WriteLine($"Cheking users in \"{group}\" group with expired passwords and sending sms message");
                adUserService.Run(group, sendMethods);
            }

            Console.WriteLine("Complete");
        }


        static void PrintConsoleMessage (User user)
        {
            if (IsUserPassExpired(user)) Console.WriteLine(user);
        }

        static void SaveToReport (User user)
        {
            AppendLineToFile(user.ToCsvLine( )+$"\t{IsUserPassExpired(user)}", _reportFile);
        }

        static async void SendSmsMessage (User user)
        {

            if (IsUserPassExpired(user) && user.Mobile != "")
            {
                var response = await _smsService.SendSmsAsync(user.Mobile, _smsText, "CRAMO");// ("37069553298", _smsText, "CRAMO");//
                AppendLineToFile(user.ToString( ), _logFile);
                AppendLineToFile(string.Format("------ Message sent: {0}{1}", response.IsSucess, ", error message: " + response.Message), _logFile);
            }
        }

        static bool IsUserPassExpired (User user)
        {
            var expireDate = DateTime.Today.AddMonths(-2).AddDays(7).Date;
            return (
                user.LastPasswordSet?.Date == expireDate
                && !user.PasswordNewerExpire
                && user.Enabled);
        }

        static async void AppendLineToFile (string stringLine, string fileName)
        {
            using (var file = new StreamWriter(fileName, append: true))
            {
                await file.WriteLineAsync(stringLine);
            }
        }
    }
}
