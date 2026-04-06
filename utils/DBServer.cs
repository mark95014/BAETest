namespace LDSTest.src.utils
{
    public class DBServer
    {
        public string serverName;
        public string userName;
        public string password;

        public DBServer(string serverName, string userName, string password)
        {
            this.serverName = serverName;
            this.userName = userName;
            this.password = password;
        }
    }


}
