namespace Common.DotNetBean
{
    public class SessionUser
    {
        public object UserId
        {
            get;
            set;
        }

        public object UserAccount
        {
            get;
            set;
        }

        public object UserPwd
        {
            get;
            set;
        }

        public object UserName
        {
            get;
            set;
        }

        public string Email { get; set; }
        public int SendApproval { get; set; }
        public string ApproverEmail { get; set; }
        public SessionUser(object userId, object userAccount, object userPwd, object userName)
        {
            this.UserId = userId;
            this.UserAccount = userAccount;
            this.UserName = userName;
            this.UserPwd = userPwd;
        }

        public SessionUser()
        {
        }
    }
}