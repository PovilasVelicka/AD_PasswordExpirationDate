using System.DirectoryServices.AccountManagement;

namespace Program
{
    public class ADUsers
    {
        private readonly PrincipalContext _context;
        public ADUsers ( )
        {
            _context = new PrincipalContext(ContextType.Domain);

        }
    }
}
