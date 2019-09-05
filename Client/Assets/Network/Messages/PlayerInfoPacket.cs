
using System;
using System.Reflection;
using UnityEngine;

public class PlayerInfoPacket : IMessage
{

    static PlayerInfoPacket()
    {
        SetTypes(MethodBase.GetCurrentMethod().DeclaringType,
            new Type[] {
                typeof(int), // ID 0
                typeof(Gun), // Gun 1
                typeof(int), // Helmet 2
                typeof(int), // Health 3
                typeof(int) // Bullets 4
            }
        );
    }

    public PlayerInfoPacket() { }

    public override void Read()
    {
        int id = (int) objects[0];
        Gun g = (Gun) objects[1];
        int helmet = (int) objects[2];
        int health = (int) objects[3];
        int bullets = (int) objects[4];

        if (!Client.instance.players.ContainsKey(id)) return;

        PlayerController pc = Client.instance.players[id];

        if (g != pc.gun) pc.SetGun(g);

        if (helmet != pc.helmet) pc.SetHelmet((ushort) helmet);

        if (pc.health > health) {

            GameObject.Instantiate(Client.instance.blood, pc.transform.position, Quaternion.identity, pc.transform);

        }

        pc.setHealth((ushort) health);

        pc.bullets = bullets;

    }

    public override void Write() { }

}
