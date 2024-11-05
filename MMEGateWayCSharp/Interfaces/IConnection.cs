using MMEGateWayCSharp.Models;
using MMEGateWayCSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MMEGateWayCSharp.Interfaces
{
    public interface IConnection
    {
        int Id { get; }
        bool IsConnected { get; }
        bool IsLoggedIn { get; }
        bool IsClosed { get; }
        string Session { get; }
        long SequenceNumber { get; }
        void SetByteOrder(ByteOrder payloadByteOrder);
        void Login(string userName, string password);
        void Login(string userName, string password, long sequenceNumber);
        void Login(string userName, string password, string session, long sequenceNumber);
        void Login(Credentials credentials);
        void Login(Credentials credentials, long sequenceNumber);
        void Login(Credentials credentials, string session, long sequenceNumber);
        void Logout();
        void Close();
        void SendMessage(IByteMessage message);
        void SetUserData(object userData);
        object GetUserData();
        void SetTrace(bool trace);
    }
}
