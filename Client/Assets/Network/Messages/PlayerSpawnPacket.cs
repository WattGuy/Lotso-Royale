
using System;
using System.Reflection;
using UnityEngine;

public class PlayerSpawnPacket : IMessage
{

    static PlayerSpawnPacket()
    {
        SetTypes(MethodBase.GetCurrentMethod().DeclaringType,
            new Type[] {
                typeof(int), // ID 0
                typeof(float), // X - Position 1
                typeof(float), // Y - Position 2
                typeof(float), // Z - Quaternion 3
                typeof(float), // W - Quaternion 4
                typeof(Gun), // Gun 5
                typeof(int) // Helmet 6
            }
        );
    }

    public PlayerSpawnPacket() { }

    public override void Read()
    {
        GameObject go = GameObject.Instantiate(Client.instance.player, new Vector2((float) objects[1], (float) objects[2]), new Quaternion(0f, 0f, (float) objects[3], (float) objects[4]));

        int id = (int) objects[0];
        Gun gun = (Gun) objects[5];
        int helmet = (int) objects[6];

        PlayerController pc = go.GetComponent<PlayerController>();
        pc.id = id;
        pc.SetHelmet((ushort) helmet);
        pc.SetGun(gun);

        if (Client.instance.id != null && Client.instance.id == id)
        {

            pc.controllable = true;
            pc.Start();

        }

        Client.instance.players.Add(id, pc);
    }

    public override void Write() { }

}
