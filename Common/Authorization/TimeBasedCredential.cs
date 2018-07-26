using System;
using System.Text;

namespace Common.Authorization
{

    /// <summary>
    /// 定义一个与时间相关的加密字凭证。
    /// 这个加密字符串包含过期时间，当过期时间大于当前时间时，即使这个加密字符串格式合法，也将视为非法。
    /// </summary>
    public abstract class TimeBasedCredential {

        private DateTime _publishTime = DateTime.Now;
        private int _duration = 24 * 60 * 60;
        private string _md5;
        protected const char SERIALIZE_SPLITTER = '\n';
        protected const byte VALUE_FIELD_START_INDEX = 2;

        /// <summary>
        /// 获取需要序列化到凭据的字段值
        /// </summary>
        /// <returns></returns>
        abstract protected string[] GetSerializeFields();

        abstract public bool Deserialize(string credential);

        /// <summary>
        /// 获取或设置凭据颁发的时间
        /// </summary>
        virtual public DateTime PublishTime {
            get { return _publishTime; }
            set { _publishTime = value; }
        }

        protected string MD5 {
            get { return _md5; }
            //set { _md5 = value; }
        }

        /// <summary>
        /// 获取或设置凭证有效期（秒），默认为 24 小时
        /// </summary>
        virtual public int Duration {
            get { return _duration; }
            set { _duration = value; }
        }

        /// <summary>
        /// 获取一个值，该值指示当前凭证是否格式完整
        /// </summary>
        public bool IsValid {
            get { 
                return _md5 == Utility.CryptographyUtility.ToMD5(GetFieldsWithoutMD5());
            }
        }

        /// <summary>
        /// 获取一个值，该值指示当前凭证是否过期
        /// </summary>
        public bool IsDue {
            get {
                DateTime overdueTime = _publishTime.AddSeconds(Duration);
                return DateTime.Now < overdueTime;
            }
        }

        /// <summary>
        /// 获取一个值，该值指示当前的加密凭证是否格式无效或者过期。
        /// </summary>
        virtual public bool IsInvalidOrOverdue {
            get {
                return IsValid == false || IsDue == false;
            }
        }

        /// <summary>
        /// 获取凭据解密后的各个字段值。
        /// 第一个字段为 MD5
        /// 第二个字段为 PublishTime
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        protected string[] GetSerializeValues(string credential) {
            try {
                string temp = Utility.CryptographyUtility.Decrypt(credential);
                string[] result = temp.Split(SERIALIZE_SPLITTER);
                _md5 = result[0];
                _publishTime = Convert.ToDateTime(result[1]);
                return result;
            } catch {
                return null;
            }
        }

        /// <summary>
        /// 将对象转换成加密凭据
        /// </summary>
        /// <returns></returns>
        public string Serialize() {
            /*StringBuilder sb = new StringBuilder();
            // 颁发时间
            sb.Append(_publishTime.ToString("yyyy-M-d H:m:s"));
            // 其它字段
            string[] otherFiles = GetSerializeFields();
            if (otherFiles != null) {
                foreach (string s in otherFiles) {
                    sb.Append(SERIALIZE_SPLITTER);
                    sb.Append(s);
                }
            }*/
            // 将所有字段MD5加密
            string allFields = GetFieldsWithoutMD5();
            string md5Field = Utility.CryptographyUtility.ToMD5(allFields);

            return Utility.CryptographyUtility.Encrypt(string.Concat(md5Field,SERIALIZE_SPLITTER,allFields));
        }

        private string GetFieldsWithoutMD5() {
            StringBuilder sb = new StringBuilder();
            // 颁发时间
            sb.Append(_publishTime.ToString("yyyy-M-d HH:mm:ss"));
            // 其它字段
            string[] otherFiles = GetSerializeFields();
            if (otherFiles != null) {
                foreach (string s in otherFiles) {
                    sb.Append(SERIALIZE_SPLITTER);
                    sb.Append(s);
                }
            }
            return sb.ToString();
        }

        


    }
}
