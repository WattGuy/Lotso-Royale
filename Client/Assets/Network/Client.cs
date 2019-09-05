using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class Client : MonoBehaviour
{

    public static Client instance;
    private static TcpClient tcp;
    public static string ip = "127.0.0.1";
    public static short port = 4296;
    public IPEndPoint IpTCP;
    public long when;
    public int? id = null;
    public int? number = null;

    public Dictionary<int, PlayerController> players = new Dictionary<int, PlayerController>();
    public Dictionary<int, GameObject> objects = new Dictionary<int, GameObject>();
    public Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

    [Header("Prefabs")]
    public GameObject player;
    public GameObject blood;
    public GameObject bullet;
    public GameObject cartridge;

    public bool connected = false;
    public Thread threadReceive;
    public Thread threadAttempting;

    void Start() {

        foreach (DestructibleType dt in Enum.GetValues(typeof(DestructibleType)).Cast<DestructibleType>())
        {

            prefabs.Add(dt.ToString(), Resources.Load<GameObject>("Objects/" + dt.ToString()));

        }

        foreach (IndestructibleType it in Enum.GetValues(typeof(IndestructibleType)).Cast<IndestructibleType>())
        {

            prefabs.Add(it.ToString(), Resources.Load<GameObject>("Objects/" + it.ToString()));

        }

        foreach (Item i in Enum.GetValues(typeof(Item)).Cast<Item>())
        {

            prefabs.Add(i.ToString(), Resources.Load<GameObject>("Items/" + i.ToString()));

        }

        instance = this;
        tcp = new TcpClient();
        tcp.NoDelay = true;

        Connect(ip, port);

    }

    public void Connect(string ip, int porttcp)
    {
        IPAddress mIp = IPAddress.Parse(ip);
        IpTCP = new IPEndPoint(mIp, porttcp);

        threadAttempting = new Thread(new ThreadStart(Attempting))
        {
            IsBackground = true
        };

        threadAttempting.Start();

        Debug.Log("Connecting...");

    }

    public void Disconnect() { Disconnect(true); }

    public void Disconnect(bool show) {

        try
        {
            if (connected)
            {
                connected = false;
                Debug.Log("Disconnected");

                if (show)
                    UnityThread.executeInUpdate(() =>
                    {

                        foreach (Transform t in GameObject.FindGameObjectWithTag("Canvas").transform)
                        {

                            if (t.name == "DisconnectScreen")
                            {

                                t.gameObject.SetActive(true);

                            }

                        }

                    });

                tcp.GetStream().Close();
                tcp.Close();
                threadReceive.Abort();
            }
        }
        catch (Exception ex) { }

    }

    private void Attempting() {

        while (true) {
            if (connected) { threadAttempting.Interrupt(); break; }

            TryConnect();

            Thread.Sleep(250);
        }

    }

    private bool TryConnect() {

        try
        {
            tcp.Connect(IpTCP);
            connected = true;

            Debug.Log("Connected");

            threadReceive = new Thread(new ThreadStart(Receive))
            {
                IsBackground = true
            };

            threadReceive.Start();

            UnityThread.executeInUpdate(() => StartCoroutine("Pinger"));

            return true;
        }
        catch (Exception ex){

            return false;

        }

    }

    private IEnumerator Pinger()
    {

        while (true)
        {
            if (!connected) break;

            new PingPacket(1).Write();

            yield return new WaitForSeconds(0.8f);
        }

    }

    private void Receive()
    {
        if (!connected) return;

        Byte[] bytes = new Byte[1024];
        int lengthl = 0;
        while (true)
        {
            try
            {
                if (!connected) return;
                NetworkStream stream = tcp.GetStream();

                int length;

                while ((length = stream.Read(bytes, lengthl, bytes.Length - lengthl)) > 0)
                {
                    KeyValuePair<byte[], List<Google.Protobuf.IMessage>> pair = ProtobufEncoding.Decode(bytes);

                    if (pair.Key.Length > 0)
                    {
                        Array.Copy(pair.Key, 0, bytes, 0, pair.Key.Length);
                        lengthl = pair.Key.Length;
                        Array.Clear(bytes, lengthl, bytes.Length - lengthl);
                    }
                    else
                    {
                        Array.Clear(bytes, 0, bytes.Length);
                        lengthl = 0;
                    }

                    foreach (Packet p in pair.Value.Cast<Packet>())
                    {
                        IMessage msg = Utils.getPacket(p);

                        if (msg != null)
                        {
                            msg.ReadSync();
                        }

                    }

                }
            }
            catch (IOException ex)
            {
                Disconnect();
            }
            catch (SocketException ex)
            {
                Disconnect();
            }
            catch (InvalidOperationException ex)
            {
                Disconnect();
            }
        }
    }

    public void Send(IMessage p)
    {
        if (!connected) return;

        try
        {

            NetworkStream stream = tcp.GetStream();
            if (stream.CanWrite)
            {

                int? id = Utils.GetID(p.GetType());
                if (id == null) return;

                byte[] b = ProtobufEncoding.Encode(new Packet() { Id = (int)id, Msg = p.ToString() }, false);

                stream.WriteAsync(b, 0, b.Length);

            }


        }
        catch (InvalidOperationException ex)
        {
            Disconnect();
        }
        catch (SocketException ex)
        {
            Disconnect();
        }

    }

    public void OnApplicationQuit()
    {

        Disconnect(false);

    }

}