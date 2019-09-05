using System;
using System.Reflection;
using UnityEngine;

public class ClientInfoPacket : IMessage
{

    static ClientInfoPacket()
    {
        SetTypes(MethodBase.GetCurrentMethod().DeclaringType, new Type[] {
            typeof(int), // ID
            typeof(int) // Objects number
        });
    }

    public ClientInfoPacket() { }

    public ClientInfoPacket(int i)
    {

        objects.Add(i);

    }

    public override void Read()
    {
        Client.instance.id = (int) objects[0];

        if (Client.instance.players.ContainsKey((int) Client.instance.id)) {

            PlayerController pc = Client.instance.players[(int) Client.instance.id];
            pc.controllable = true;
            pc.Start();

        }

        Client.instance.number = (int) objects[1];

        if (Client.instance.number != null && Client.instance.objects.Count >= Client.instance.number) {

            GameObject go = GameObject.FindGameObjectWithTag("LoadingScreen");

            if (go == null) return;

            go.SetActive(false);

        }

    }

    public override void Write() {}

}
