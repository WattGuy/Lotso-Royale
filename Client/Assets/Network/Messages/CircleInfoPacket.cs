
using System;
using System.Reflection;
using UnityEngine;

public class CircleInfoPacket : IMessage
{

    static CircleInfoPacket()
    {
        SetTypes(MethodBase.GetCurrentMethod().DeclaringType,
            new Type[] {
                typeof(float), // X - Position 0
                typeof(float), // Y - Position 1
                typeof(float) // Scale 2
            }
        );
    }

    public CircleInfoPacket() { }

    public override void Read()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Circle");

        if (go == null) return;

        go.transform.position = new Vector3((float) objects[0], (float) objects[1], 0.005f);
        go.transform.localScale = new Vector3((float) objects[2], (float) objects[2], 1f);
    }

    public override void Write() { }

}
