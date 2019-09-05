using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ObjectDespawnPacket : IMessage
{

    static ObjectDespawnPacket()
    {
        SetTypes(MethodBase.GetCurrentMethod().DeclaringType, new Type[] {
            typeof(int), // ID 0
            typeof(ObjectType) // Type of object 1
        });
    }

    public ObjectDespawnPacket() { }

    public override void Read()
    {
        int id = (int) objects[0];
        ObjectType ot = (ObjectType) objects[1];

        if (ot == ObjectType.PLAYER) {

            if (!Client.instance.players.ContainsKey(id)) return;

            bool was = false;
            if (Client.instance.id != null && Client.instance.id == id) {
                was = true;

                foreach (Transform t in GameObject.FindGameObjectWithTag("Canvas").transform)
                {

                    if (t.name == "DeathScreen")
                    {

                        t.gameObject.SetActive(true);

                    }

                }

            }

            GameObject.Destroy(Client.instance.players[id].gameObject);
            if (!was) Client.instance.players.Remove(id);

        } else if (ot != ObjectType.NONE) {

            if (!Client.instance.objects.ContainsKey(id)) return;

            GameObject.Destroy(Client.instance.objects[id]);
            Client.instance.objects.Remove(id);

        }

    }

    public override void Write() {}

}
