using Common.Utility;

namespace Common.DotNetBean
{
    public class RequestSession
    {
        private static string SESSION_USER = "SESSION_USER";

        public static void AddSessionUser(SessionUser user)
        {
            SessionHelper.Set(SESSION_USER, user);
        }

        public static SessionUser GetSessionUser()
        {
            return (SessionUser)SessionHelper.Get(SESSION_USER);
        }
    }
}