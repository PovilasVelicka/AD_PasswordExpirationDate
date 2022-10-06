
using BaltnetaSmsApi;
using System;
using System.Configuration;
using System.IO;

namespace PasswordExpire
{

    internal class Program
    {
        static readonly string _logFile;
        const string LOG_FILE_NAME = "password_expiration.log";
        private static readonly SmsService _smsService;
        private static readonly string _smsText;
        static Program ( )
        {
            _logFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly( ).Location);
            if (!Directory.Exists(_logFile)) Directory.CreateDirectory(_logFile);
            _logFile += Path.DirectorySeparatorChar + LOG_FILE_NAME;
            _smsText = ConfigurationManager.AppSettings["smsMessage"];
            _smsService = new SmsService(
                apiKey: ConfigurationManager.AppSettings["apiKey"],
                login: ConfigurationManager.AppSettings["login"]);

        }
        static void Main (string[ ] args)
        {
            var adUserService = new AdUserService("LT_All Users");

            SendMessage sendMethods = PrintConsoleMessage;
            sendMethods += SendSmsMessage;


            adUserService.SendMessages(sendMethods);

        }

        static void PrintConsoleMessage (User user)
        {
            Console.WriteLine(user);
        }

        static async void SendSmsMessage (User user)
        {
            var response = await _smsService.SendSmsAsync (user.Mobile, _smsText, "CRAMO");// ("37069553298", _smsText, "CRAMO");//
            SaveLogFile(user.ToString( ));
            SaveLogFile(string.Format("------ Message sent: {0}{1}", response.IsSucess, ", error message: " + response.Message));
        }

        static async void SaveLogFile (string stringLine)
        {
            using (var file = new StreamWriter(_logFile, append: true))
            {
                await file.WriteLineAsync(stringLine);
            }
        }
    }
}
