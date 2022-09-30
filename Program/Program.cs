// See https://aka.ms/new-console-template for more information
using System.DirectoryServices.AccountManagement;

Console.WriteLine("Hello, World!");
static PrincipalContext ctx;
    
    ctx = new PrincipalContext(ContextType.Domain);

foreach (var member in GetGroup("LT_All Users").Members)
{
    var user = GetUser(member.UserPrincipalName);
    
}


static GroupPrincipal GetGroup (string groupName)
{
   
        using (var userPrinciple = new GroupPrincipal(ctx))
        {
            userPrinciple.SamAccountName = groupName;

            using (var search = new PrincipalSearcher(userPrinciple))
            {
                GroupPrincipal group = (GroupPrincipal)search.FindOne( );
         
                if (group == null)
                {
                    throw new Exception("not found directory");
                }
               return group; // auth'ed user
            }
        }
    
}

static UserPrincipal GetUser(string userName)
{
   

        using (var userPrinciple = new UserPrincipal(ctx))
        {
            userPrinciple.SamAccountName = userName;

            using (var search = new PrincipalSearcher(userPrinciple))
            {
                UserPrincipal user = (UserPrincipal)search.FindOne( );
                if (user == null)
                {
                    throw new Exception("user authenticated but not found in directory");
                }
                return user; // auth'ed user
            }
        }
    
}

//using (var userPrinciple = new UserPrincipal(ctx))
//{
//    using (var ctx = new PrincipalContext(ContextType.Domain))
//    {
//        using (var groupPrinciple = new GroupPrincipal(ctx))
//        {
//            groupPrinciple.SamAccountName = "LT_All_Users";
//            using (var search = new PrincipalSearcher(groupPrinciple))
//            {
//                var member_list = (GroupPrincipal)search.FindOne( );
//            }
//            // member_list contains all the users of a group. 
//            // I cache these in a Dictionary for faster group membership checks
//        }
//    }
//}