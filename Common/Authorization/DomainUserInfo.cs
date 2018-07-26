namespace Common.Authorization {
    public class DomainUserInfo {

        public string ADAccount { get; set; }
        public string Office { get; set; }
        public string JobTitle { get; set; }
        public string EmployeeID { get; set; }
        public string DistinguishedName { get; set; }
        public string ManagerDescription { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ManagerName {
            get {
                if (string.IsNullOrEmpty(ManagerDescription)) {
                    return null;
                }
                try {
                    return ManagerDescription.Substring(3, ManagerDescription.IndexOf(',') - 3);
                } catch {
                    return ManagerDescription;
                } 
            }
        }

        public override string ToString() {
            return string.Format(
                "{0}\t{1}\t{2}\t{3}\t{4}",
                ADAccount, FirstName + " " + LastName, JobTitle, Office, ManagerName
                );
        }


    }
}
