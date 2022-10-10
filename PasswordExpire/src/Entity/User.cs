using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            return String.Format("Date: {4}, Guid: {0}, Name: {1} {2}, Phone: {3}", guid, FirstName, LastName, Mobile, DateTime.Now.ToString("yyyy-MM-dd") );
        }

    }
}
