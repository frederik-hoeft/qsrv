namespace qsrv
{
    /// <summary>
    /// Account info eventually shared with the clients
    /// </summary>
    public class AccountInfo
    {
        public string Name;
        public string Occupation;
        public string Info1;
        public string Info2;
        public string Info3;
        public string Info4;
        public string Info5;
        public string Info6;
        public string Info7;
        public string Info8;
        public string Info9;
        public string Info10;
        public string Location;
        public int Radius;
        public string UserId;
        public string Email;
        public bool IsVisible;
        public bool ShowLog;

        public AccountInfo(string name, string occupation, string info1, string info2, string info3, string info4, string info5, string info6, string info7, string info8, string info9, string info10, string location, int radius, string userid, string email, bool isVisible, bool showLog)
        {
            Name = name;
            Occupation = occupation;
            Info1 = info1;
            Info2 = info2;
            Info3 = info3;
            Info4 = info4;
            Info5 = info5;
            Info6 = info6;
            Info7 = info7;
            Info8 = info8;
            Info9 = info9;
            Info10 = info10;
            Location = location;
            Radius = radius;
            UserId = userid;
            Email = email;
            IsVisible = isVisible;
            ShowLog = showLog;
        }

        public AccountInfo()
        {
        }
    }
}