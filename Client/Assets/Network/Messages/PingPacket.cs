using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class PingPacket : IMessage
{

    static PingPacket()
    {
        SetTypes(MethodBase.GetCurrentMethod().DeclaringType, new Type[] {
            typeof(byte)
        });
    }

    public PingPacket() { }

    public PingPacket(byte i)
    {

        objects.Add(i);

    }

    public override void Read()
    {
        Text ping = GameObject.FindGameObjectWithTag("Ping").GetComponent<Text>();
        ping.text = "Задержка: " + ((int) (DateTimeOffset.Now.ToUnixTimeMilliseconds() - Client.instance.when)) + " мс";
    }

    public override void Write()
    {
        Client.instance.when = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Client.instance.Send(this);
    }

}
