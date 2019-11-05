using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SmartwallPackage.Utils
{
    class BufferStateObject
    {
        public byte[] buffer = new byte[8192];
    }

    /// <summary>
    /// Class to facilitate simple tcp messaging. WARNING this class does not contain message merging or Queueing, 
    /// this means that when dealing with large messages or high trafic, there is no safegaurd agenst messages getting chopped up or merged by TCP.
    /// This class queues messages to maintain transmission order.
    /// </summary>
    public class TcpConnection : IDisposable
    {
        public delegate void DisconnectedHandler(string exitMessage);
        public event DisconnectedHandler Disconnected;
        public delegate void TcpMessageHandler(int byteCount, byte[] data);
        public event TcpMessageHandler MessageRecieved;
        private bool Sending;
        private Queue<byte[]> _MessageQueue = new Queue<byte[]>();

        TcpClient _Connection;

        public TcpConnection(TcpClient connection)
        {
            _Connection = connection;
            ListenForMessages();
        }

        public void SendMessage(byte[] data)
        {
            if (Sending)
            {
                _MessageQueue.Enqueue(data);
            }
            else
            {
                try
                {
                    _Connection.Client.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, _Connection);
                    Sending = true;
                }
                catch (SocketException sEx)
                {
                    if (!_Connection.Connected)
                    {
                        Disconnected.Invoke(sEx.Message);
                    }
                }
            }
        }
        private void SendCallback(IAsyncResult ar)
        {
            _Connection.Client.EndSend(ar);
            Sending = false;
            if(_MessageQueue.Count > 0)
            {
                SendMessage(_MessageQueue.Dequeue());
            }
        }

        private void ListenForMessages()
        {
            try
            {
                BufferStateObject state = new BufferStateObject();
                _Connection.Client.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, RecieveMessage, state);
            }
            catch (SocketException sEx)
            {
                if (_Connection.Connected)
                {
                    Task.Delay(500).ContinueWith(t=>ListenForMessages());
                }
                else
                {
                    Disconnected.Invoke(sEx.Message);
                }
            }
        }
        private void RecieveMessage(IAsyncResult ar)
        {
            int               recievedBytes = _Connection.Client.EndReceive(ar);
            BufferStateObject state         = ar.AsyncState as BufferStateObject;
            byte[]            data          = new byte[recievedBytes];
            Array.Copy(state.buffer, data, recievedBytes);
            MainThreadDispatcher.Instance.RunOnMainThread(() => InvokeRecievedEvent(recievedBytes, data));
            ListenForMessages();
        }
        private void InvokeRecievedEvent(int byteCount, byte[] data)
        {
            MessageRecieved.Invoke(byteCount, data);
        }

        public void Dispose()
        {
            _Connection.Dispose();
            Disconnected.Invoke("Being Disposed");
        }
    }
}