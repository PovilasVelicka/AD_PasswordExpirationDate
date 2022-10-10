using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;

namespace PasswordExpire
{
    internal delegate void SendMessage (User user);

    internal class AdUserService
    {

        private readonly PrincipalContext _context;

        public AdUserService ( )
        {
            _context = new PrincipalContext(ContextType.Domain);
        }

        public async Task<IReadOnlyList<User>> GetGroupUsersAsync (string groupName)
        {

            var users = new List<User>( );
            var groupTask =   GetGroupAsync(groupName);
            
            foreach (var groupMember in groupTask.Result.Members)
            {
                var user = await GetUserAsync(groupMember.SamAccountName);
                // var expireDate = user.LastPasswordSet?.AddMonths(2).AddDays(10).Date;
                users.Add(user);
            }
            return users;
        }

        private  Task<GroupPrincipal> GetGroupAsync (string groupName)
        {
            return Task<GroupPrincipal>.Run(( ) =>
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
            });

        }

        private async Task<User> GetUserAsync (string userName)
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

                        return new User
                        {
                            guid = user.Guid,
                            FirstName = user.GivenName,
                            LastName = user.Surname,
                            Mobile = GetUserMobile(user),
                            Enabled = user.Enabled ?? false,
                            LastPasswordSet = user.LastPasswordSet,
                            LastLogon = user.LastLogon,
                            PasswordNewerExpire = user.PasswordNeverExpires,
                        };
                    }
                }
          
        }

        private string GetUserMobile(UserPrincipal user)
        {

            using (DirectoryEntry de = user.GetUnderlyingObject( ) as DirectoryEntry)
            {
                if (de != null)
                {
                    return de.Properties["mobile"].Value as string;

                }
                return "";
            }

        }
    }
}
