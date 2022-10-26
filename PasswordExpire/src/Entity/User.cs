using System;

namespace PasswordExpire
{
    internal class User
    {
        public Guid? guid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public bool Enabled { get; set; }
        public DateTime? LastPasswordSet { get; set; }
        public DateTime? LastLogon { get; set; }
        public bool PasswordNewerExpire { get; set; }
        public override string ToString ( )
        {

            return String.Format("Date: {0}, Name: {1}, Phone: {2}, LastPasswordSetDate: {3}, UserEnabled: {4}", ToCsvLine( ).Split('\t'));
        }
        public string ToCsvLine (char separator = '\t')
        {
            return String.Format("{3}\t{0} {1}\t{2}\t{4}\t{5}",
              
                FirstName,
                LastName,
                Mobile,
                DateTime.Now.ToString("yyyy-MM-dd"),
                LastPasswordSet?.ToString("yyyy-MM-dd hh:mm:ss"),
                Enabled);
        }
    }
}
