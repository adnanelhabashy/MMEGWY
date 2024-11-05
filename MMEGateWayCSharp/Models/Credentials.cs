using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Models
{
    /// <summary>
    /// Represents user credentials consisting of a username and password.
    /// </summary>
    public class Credentials
    {
        private readonly string _userName;
        private readonly string _password;
        /// <summary>
        /// Initializes a new instance of the Credentials class with the specified username and password.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        public Credentials(string userName, string password)
        {
            _userName = userName;
            _password = password;
        }
        /// <summary>
        /// Gets the username.
        /// </summary>
        public string UserName => _userName;
        /// <summary>
        /// Gets the password.
        /// </summary>
        public string Password => _password;
    }
}
