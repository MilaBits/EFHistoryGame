using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SmartwallPackage.Language;
using UnityEngine;

namespace SmartwallPackage.Utils
{
    /// <summary>
    /// This Class serves at the gateway to the central application. 
    /// Currently it is only used for the central to inform a game what language file it should use.
    /// </summary>
    public class CentralGateway : MonoBehaviour
    {
        public static CentralGateway Instance;
        TcpConnection _Connection;


        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("CentralGateway | Awake | A second CentralGateway was loaded, maybe you should make a initialisation scene?");
                Destroy(this);
            }
        }

        private void Start()
        {
            ConnectToCentral();
        }

        private void ConnectToCentral()
        {
            TcpClient temp = new TcpClient();
            temp.BeginConnect(IPAddress.Loopback.ToString(), 2237, ConnectCallback, temp);
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            TcpClient temp = ar.AsyncState as TcpClient;
            temp.EndConnect(ar);
            if(_Connection == null)
            {
                _Connection = new TcpConnection(temp);
            }
            else
            {
                lock (_Connection)
                {
                    _Connection = new TcpConnection(temp);
                }
            }
        }

        private void MessageFromCentral(int dataLength, byte[] data)
        {
            if(dataLength < 1)
            {
                return;
            }
            ProcessMessageData(data);
        }

        private void ProcessMessageData(byte[] data)
        {
            int commandSize = 1;
            switch(data[0]){
                case 1:
                    commandSize = 3;
                    if (data.Length >= commandSize)
                    {
                        LanguageController.Instance.LoadLanguage(Encoding.ASCII.GetString(data, 1, 2));
                    }
                    break;
            }
            if(data.Length > commandSize)
            {
                List<byte> leftovers = new List<byte>();
                leftovers.AddRange(data);
                leftovers.RemoveRange(0, 3);
                ProcessMessageData(leftovers.ToArray());
            }
        }
    }
}
