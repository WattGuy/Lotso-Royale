
using System;
using System.Reflection;
using UnityEngine;

public class PlayerMovePacket : IMessage
{

    static PlayerMovePacket()
    {
        SetTypes(MethodBase.GetCurrentMethod().DeclaringType,
            new Type[] {
                typeof(int), // ID 0
                typeof(float), // X - Position 1
                typeof(float), // Y - Position 2
                typeof(float), // Z - Quaternion 3
                typeof(float) // W - Quaternion 4
            }
        );
    }

    public PlayerMovePacket() { }

    public PlayerMovePacket(int id, float x, float y, float z, float w)
    {

        objects.Add(id);
        objects.Add(x);
        objects.Add(y);
        objects.Add(z);
        objects.Add(w);

    }

    public override void Read()
    {
        int id = (int) objects[0];
        if (!Client.instance.players.ContainsKey(id)) return;

        PlayerController pc = Client.instance.players[id];

        pc.lastPosition = pc.transform.position;
        pc.lastRotation = pc.transform.rotation;

        pc.toPosition = new Vector2((float) objects[1], (float) objects[2]);
        pc.toRotation = new Quaternion(0f, 0f, (float) objects[3], (float) objects[4]);

    }

    public override void Write()
    {

        Client.instance.Send(this);

    }

}
