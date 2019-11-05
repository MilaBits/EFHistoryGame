using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SmartwallPackage.Utils;
using UnityEngine;

namespace SmartwallPackage.SmartwallInput
{
    /// <summary>
    /// This class makes connection with the DSoft, reads the incoming blob data and processes it into a List of Blob objects
    /// The List of Blob objects is publicly and staticly availible so other classes can easely acces it from anywhere.
    /// The class also offers a NewBlobData event that will send a list of the new Blobs recieved.
    /// Take into account that you will not recieve new data every frame, hence the public List.
    /// </summary>
    public class BlobDetectionGateway : MonoBehaviour
    {
        public delegate void NewBlobDataHandler(List<Blob> blobs);
        public static event NewBlobDataHandler NewBlobData;

        private static BlobDetectionGateway _Instance;

        /// <summary>
        /// The most recent blobs detected by the detection software.
        /// </summary>
        public static List<Blob> Blobs = new List<Blob>();
        TcpConnection _Connection;
        private int _BlobDataSize = 20;
        private List<byte> _TotalData = new List<byte>();

        private void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogWarning("BlobDetectionGateway | Awake | A second BlobDetectionGateway was loaded, maybe you should make a initialisation scene?");
                Destroy(this);
            }
        }

        private void Start()
        {
            ConnectToDSoft();
        }

        private void ConnectToDSoft()
        {
            TcpClient temp = new TcpClient();
            temp.BeginConnect(IPAddress.Loopback.ToString(), 3000, ConnectCallback, temp);
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            TcpClient temp = ar.AsyncState as TcpClient;
            temp.EndConnect(ar);
            if (_Connection == null)
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
            _Connection.MessageRecieved += MessageFromDSoft;
            _Connection.Disconnected    += ConnectionLost;
        }

        //checks if the message was an actual data transmission
        private void MessageFromDSoft(int dataLength, byte[] data)
        {
            //Is message empty? this can happen when a connection closes.
            if (dataLength < 1)
            {
                return;
            }
            ProcessMessageData(data); //is actual message, send to data processing.
        }

        //Check if we have a full data transmission and merges or seperates the data as needed to undo possible merger and seperation created by the TCP protocol.
        private void ProcessMessageData(byte[] data)
        {
            byte nrOfBlobs;
            if (_TotalData.Count == 0) //Are we expecting a new batch of data?
            {
                if (!Encoding.ASCII.GetString(data, 0, 3).Equals("BDT")) //does it have the starting header?
                {
                    Debug.LogWarning("BlobDetectionGateway | MessageFromDSoft | Expected start of new transmition but recieved intermitten segment.");
                    return;
                }
                nrOfBlobs = data[3];
            }
            else //then we are expecting a followup segment of data
            {
                if (Encoding.ASCII.GetString(data, 0, 3).Equals("BDT"))
                {
                    Debug.LogWarning("BlobDetectionGateway | MessageFromDSoft | Recieving new transmition while old one is not complete.");
                    _TotalData.Clear();
                    nrOfBlobs = data[3];
                }
                else
                {
                    nrOfBlobs = _TotalData[3];
                }
            }

            _TotalData.AddRange(data);

            if (_TotalData.Count >= nrOfBlobs * _BlobDataSize + 4) //do we have all the data we need now?
            {
                LoadNewBlobsFromData(nrOfBlobs,_TotalData.GetRange(4, nrOfBlobs * _BlobDataSize).ToArray());

                List<byte> leftovers = new List<byte>();
                if (_TotalData.Count > nrOfBlobs * _BlobDataSize + 4) //do we have part of the next transmission at the end?
                {
                    byte[] temp = new byte[_TotalData.Count - (nrOfBlobs * _BlobDataSize + 4)];
                    Array.Copy(_TotalData.ToArray(), nrOfBlobs * _BlobDataSize + 4, temp, 0, temp.Length);
                    leftovers.AddRange(temp);
                }
                _TotalData.Clear();
                if (leftovers.Count > 0) //if there are leftovers, aka the start of a next transmission, recieve that segment as new.
                {
                    MessageFromDSoft(leftovers.Count, leftovers.ToArray());
                }
            }
        }

        //Actually loading the blob data so it can be used
        private void LoadNewBlobsFromData(int nrOfBlobs, byte[] data)
        {
            Blobs.Clear();
            for (int i = 0; i < nrOfBlobs; i++)
            {
                //one blob is 20 bytes of data; offset of message number + ofset of value in data segment
                int   id = BitConverter.ToInt32(data, i * _BlobDataSize);
                float x  = BitConverter.ToSingle(data, i * _BlobDataSize + 4);
                float y  = BitConverter.ToSingle(data, i * _BlobDataSize + 8);
                y = 1 - y; //Unity's Y axis goes from bottom to top while normal images go from top to bottum so we invert it.
                float width  = BitConverter.ToSingle(data, i * _BlobDataSize + 12);
                float height = BitConverter.ToSingle(data, i * _BlobDataSize + 16);
                Blobs.Add(new Blob(id, x, y, width, height));
            }
            if (NewBlobData != null)
            {
                NewBlobData.Invoke(Blobs);
            }
        }

        private void ConnectionLost(string message)
        {
            Debug.Log("BlobDetectionGateway | ConnectionLost | The connection to DSoft was lost, trying to reconnect.");
            Invoke("ConnectToDSoft", 3f);
        }
    }
}
