using System;
using System.Reflection;
using UnityEngine;

public class ObjectSpawnPacket : IMessage
{

    static ObjectSpawnPacket()
    {
        SetTypes(MethodBase.GetCurrentMethod().DeclaringType, new Type[] {
            typeof(int), // ID 0
            typeof(ObjectType), // Type of object 1
            typeof(string), // DestType | IndestType | ItemType 2
            typeof(float), // X - Position 3 
            typeof(float), // Y - Position 4
            typeof(int) // Health for destructible object 5
        });
    }

    public ObjectSpawnPacket() { }

    public override void Read()
    {
        int id = (int) objects[0];
        ObjectType ot = (ObjectType) objects[1];
        string type = (string) objects[2];

        if (!Client.instance.prefabs.ContainsKey(type)) return;
        GameObject prefab = Client.instance.prefabs[type];

        GameObject go = GameObject.Instantiate(prefab, new Vector2((float)objects[3], (float)objects[4]), Quaternion.identity);
        Client.instance.objects.Add(id, go);

        if (ot == ObjectType.DESTRUCTIBLE) {

            Destructible d = go.GetComponent<Destructible>();
            d.id = id;
            d.SetHealth((int) objects[5]);

        } else if (ot == ObjectType.ITEM) {

            ItemController ic = go.GetComponent<ItemController>();
            ic.id = id;

        }

        if (Client.instance.number != null && Client.instance.objects.Count >= Client.instance.number)
        {

            GameObject go2 = GameObject.FindGameObjectWithTag("LoadingScreen");

            if (go2 == null) return;

            go2.SetActive(false);

        }

    }

    public override void Write() {}

}
