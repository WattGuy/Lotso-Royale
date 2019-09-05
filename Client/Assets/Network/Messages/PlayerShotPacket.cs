using System;
using System.Reflection;
using UnityEngine;

public class PlayerShotPacket : IMessage
{

    static PlayerShotPacket()
    {
        SetTypes(MethodBase.GetCurrentMethod().DeclaringType, new Type[] {
            typeof(int), // ID 0
            typeof(ObjectType), // Type of object 1
            typeof(int), // ID of object 2
            typeof(float) // Degrees 3
        });
    }

    public PlayerShotPacket() { }

    public PlayerShotPacket(int id, ObjectType t, int oid, float degrees)
    {

        objects.Add(id);
        objects.Add(t);
        objects.Add(oid);
        objects.Add(degrees);

    }

    public override void Read()
    {
        int id = (int) objects[0];

        if (!Client.instance.players.ContainsKey(id)) return;

        PlayerController pc = Client.instance.players[id];
        if (!(Client.instance.id != null && Client.instance.id == id)) pc.playGun();

        ObjectType ot = (ObjectType) objects[1];
        int oid = (int) objects[2];

        if (ot == ObjectType.DESTRUCTIBLE) {

            if (Client.instance.objects.ContainsKey(oid)) {

                GameObject go = Client.instance.objects[oid];
                Destructible d = go.GetComponent<Destructible>();

                if (d != null) {

                    d.SetHealth(d.health - 1);

                }

            }

        } else if (ot == ObjectType.PLAYER) {

            if (Client.instance.players.ContainsKey(oid))
            {

                PlayerController target = Client.instance.players[oid];
                target.setHealth((ushort) (target.health - 1));

            }

        }

    }

    public override void Write() { Client.instance.Send(this); }

}
