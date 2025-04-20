using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPReceive : MonoBehaviour
{
   [SerializeField] Thread receiveThread;
   [SerializeField] UdpClient client;
   [SerializeField] int port = 12345; // Port to listen on
   public string receivedData = "";

   private void Start()
   {
       receiveThread = new Thread(ReceiveData) { IsBackground = true };
       receiveThread.Start();
   }

   private void ReceiveData()
   {
       client = new UdpClient(port);
       while (true)
       {
           try
           {
               IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
               byte[] data = client.Receive(ref anyIP);
               receivedData = Encoding.UTF8.GetString(data);
               Debug.Log("Received UDP Data: " + receivedData);
           }
           catch (System.Exception e)
           {
               Debug.LogError(e.ToString());
           }
       }
   }

   private void OnApplicationQuit()
   {
       if (receiveThread != null) receiveThread.Abort();
       if (client != null) client.Close();
   }

}