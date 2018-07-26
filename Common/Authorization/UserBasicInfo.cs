using System;

namespace Common.Authorization {

    /// <summary>
    /// �û���Ϣʵ�壬ֻ������������û���Ϣ��
    /// ���ʵ��ͨ���������ڿͻ��ˡ�
    /// </summary>
    public class UserBasicInfo : TimeBasedCredential {

        private int _userID;
        private string _userName; //AD
        private string _nickName;
        //private string _pwd;
        private string _email;
        private bool _isAdmin;
        
        //private const char SERIALIZE_SP = '\n';

        public UserBasicInfo() { }

        public UserBasicInfo(int id, string userName, string sEmail, string sNickName, bool isAdmin)
        {
            _userID = id;
            _userName = userName;
            _nickName = sNickName;
            _email = sEmail;
            _isAdmin = isAdmin;
        }

        /// <summary>
        /// ��ȡ�������û���AD�û���
        /// </summary>
        public string AdName {
            get { return _userName; }
            set { _userName = value; }
        }

        /// <summary>
        /// ��ȡ�������û���ID
        /// </summary>
        public int UserID {
            get { return _userID; }
            set { _userID = value; }
        }

        /// <summary>
        /// ��ȡ�������û�����
        /// </summary>
        //public string Password {
        //    get { return _pwd; }
        //    set { _pwd = value; }
        //}

        /// <summary>
        /// ��ȡ�������û�Email
        /// </summary>
        public string Email {
            get { return _email; }
            set { _email = value; }

        }

        public bool IsAdmin
        {
            get { return _isAdmin; }
            set { _isAdmin = value; }
        }

        public string NickName
        {
            get { return _nickName; }
            set { _nickName = value; }
        }

        //public string ScreenName {
        //    get {
        //        return string.Format(
        //            "{0} <{1}>",
        //            NickName,
        //            UserName
        //            );
        //    }
        //}

        protected override string[] GetSerializeFields() {
            return new string[] {
                UserID.ToString(),
                AdName,
                NickName,
                Email,
                IsAdmin.ToString()
            };
        }
        
        public override bool Deserialize(string credential) {

            try {
                string[] values = base.GetSerializeValues(credential);
                int i = VALUE_FIELD_START_INDEX;
                this.UserID = int.Parse(values[i++]);
                this.AdName = values[i++];
                this.NickName = values[i++];
                //this.Password = values[VALUE_FIELD_START_INDEX + 3];
                this.Email = values[i++];
                this.IsAdmin = Boolean.Parse(values[i++]);
                
                return true;
            } catch {
                return false;
            }
        }
    }
}
