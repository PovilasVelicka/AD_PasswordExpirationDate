using System;
using System.Collections.Generic;
using System.Linq;
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
        public override string ToString ( )
        {
            return String.Format("Guid: {0}, Name: {1} {2}, Phone: {3}", guid, FirstName, LastName, Mobile);
        }
    }
}
