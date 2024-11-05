using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Models
{
    public class Credentials
    {
        private readonly string _userName;
        private readonly string _password;

        public Credentials(string userName, string password)
        {
            _userName = userName;
            _password = password;
        }

        public string UserName => _userName;
        public string Password => _password;
    }
}
