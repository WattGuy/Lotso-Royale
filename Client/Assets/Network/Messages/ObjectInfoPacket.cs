using System;
using System.Reflection;
using UnityEngine;

public class ObjectInfoPacket : IMessage
{

    static ObjectInfoPacket()
    {
        SetTypes(MethodBase.GetCurrentMethod().DeclaringType, new Type[] {
            typeof(int), // ID
            typeof(int) // Health
        });
    }

    public ObjectInfoPacket() { }

    public ObjectInfoPacket(int i)
    {

        objects.Add(i);

    }

    public override void Read()
    {
        int id = (int) objects[0];

        if (!Client.instance.objects.ContainsKey(id)) return;

        GameObject go = Client.instance.objects[id];

        if (go.GetComponent<Destructible>() == null) return;

        Destructible d = go.GetComponent<Destructible>();

        d.SetHealth((int) objects[1]);

    }

    public override void Write() {}

}
