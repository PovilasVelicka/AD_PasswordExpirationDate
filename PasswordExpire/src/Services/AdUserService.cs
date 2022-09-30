using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace PasswordExpire
{
    internal delegate void SendMessage (User user);

    internal class AdUserService
    {

        private readonly PrincipalContext _context;
        private readonly string _groupName;


        public AdUserService (string groupName)
        {
            _groupName = groupName;
            _context = new PrincipalContext(ContextType.Domain);
        }

        public void SendMessages (SendMessage methods)
        {
            foreach (var groupMember in GetGroup(_groupName).Members)
            {
                var user = GetUser(groupMember.SamAccountName);
                var expireDate = user.LastPasswordSet.GetValueOrDefault(DateTime.Now).AddMonths(2).AddDays(10).Date;

                if (expireDate == DateTime.Now.Date)
                {
                    if (methods != null)
                    {
                        using (DirectoryEntry de = user.GetUnderlyingObject( ) as DirectoryEntry)
                            if (de != null)
                            {
                                var mobile = de.Properties["mobile"].Value as string;
                                if (!string.IsNullOrWhiteSpace(mobile))
                                {
                                    methods.Invoke(new User
                                    {
                                        guid = user.Guid,
                                        FirstName = user.GivenName,
                                        LastName = user.Surname,
                                        Mobile = mobile,
                                    });
                                }
                            }
                    }
                }
            }
        }

        private GroupPrincipal GetGroup (string groupName)
        {
            using (var userPrinciple = new GroupPrincipal(_context))
            {
                userPrinciple.SamAccountName = groupName;

                using (var search = new PrincipalSearcher(userPrinciple))
                {
                    GroupPrincipal group = (GroupPrincipal)search.FindOne( );

                    if (group == null)
                    {
                        throw new Exception("not found directory");
                    }
                    return group;
                }
            }
        }

        private UserPrincipal GetUser (string userName)
        {
            using (var userPrinciple = new UserPrincipal(_context))
            {
                userPrinciple.SamAccountName = userName;

                using (var search = new PrincipalSearcher(userPrinciple))
                {
                    UserPrincipal user = (UserPrincipal)search.FindOne( );
                    if (user == null)
                    {
                        throw new Exception("user authenticated but not found in directory");
                    }
                    return user;
                }
            }
        }
    }
}
