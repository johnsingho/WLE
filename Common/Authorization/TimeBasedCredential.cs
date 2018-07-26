using System;
using System.Text;

namespace Common.Authorization
{

    /// <summary>
    /// ����һ����ʱ����صļ�����ƾ֤��
    /// ��������ַ�����������ʱ�䣬������ʱ����ڵ�ǰʱ��ʱ����ʹ��������ַ�����ʽ�Ϸ���Ҳ����Ϊ�Ƿ���
    /// </summary>
    public abstract class TimeBasedCredential {

        private DateTime _publishTime = DateTime.Now;
        private int _duration = 24 * 60 * 60;
        private string _md5;
        protected const char SERIALIZE_SPLITTER = '\n';
        protected const byte VALUE_FIELD_START_INDEX = 2;

        /// <summary>
        /// ��ȡ��Ҫ���л���ƾ�ݵ��ֶ�ֵ
        /// </summary>
        /// <returns></returns>
        abstract protected string[] GetSerializeFields();

        abstract public bool Deserialize(string credential);

        /// <summary>
        /// ��ȡ������ƾ�ݰ䷢��ʱ��
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
        /// ��ȡ������ƾ֤��Ч�ڣ��룩��Ĭ��Ϊ 24 Сʱ
        /// </summary>
        virtual public int Duration {
            get { return _duration; }
            set { _duration = value; }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ǰƾ֤�Ƿ��ʽ����
        /// </summary>
        public bool IsValid {
            get { 
                return _md5 == Utility.CryptographyUtility.ToMD5(GetFieldsWithoutMD5());
            }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ǰƾ֤�Ƿ����
        /// </summary>
        public bool IsDue {
            get {
                DateTime overdueTime = _publishTime.AddSeconds(Duration);
                return DateTime.Now < overdueTime;
            }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ǰ�ļ���ƾ֤�Ƿ��ʽ��Ч���߹��ڡ�
        /// </summary>
        virtual public bool IsInvalidOrOverdue {
            get {
                return IsValid == false || IsDue == false;
            }
        }

        /// <summary>
        /// ��ȡƾ�ݽ��ܺ�ĸ����ֶ�ֵ��
        /// ��һ���ֶ�Ϊ MD5
        /// �ڶ����ֶ�Ϊ PublishTime
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
        /// ������ת���ɼ���ƾ��
        /// </summary>
        /// <returns></returns>
        public string Serialize() {
            /*StringBuilder sb = new StringBuilder();
            // �䷢ʱ��
            sb.Append(_publishTime.ToString("yyyy-M-d H:m:s"));
            // �����ֶ�
            string[] otherFiles = GetSerializeFields();
            if (otherFiles != null) {
                foreach (string s in otherFiles) {
                    sb.Append(SERIALIZE_SPLITTER);
                    sb.Append(s);
                }
            }*/
            // �������ֶ�MD5����
            string allFields = GetFieldsWithoutMD5();
            string md5Field = Utility.CryptographyUtility.ToMD5(allFields);

            return Utility.CryptographyUtility.Encrypt(string.Concat(md5Field,SERIALIZE_SPLITTER,allFields));
        }

        private string GetFieldsWithoutMD5() {
            StringBuilder sb = new StringBuilder();
            // �䷢ʱ��
            sb.Append(_publishTime.ToString("yyyy-M-d HH:mm:ss"));
            // �����ֶ�
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
