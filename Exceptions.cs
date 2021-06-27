using System;

namespace MonitorNotas
{
    public class LoginException : Exception
    {
        public LoginException()
        {

        }

        public LoginException(string msg): base(msg)
        {

        }
    }
}
