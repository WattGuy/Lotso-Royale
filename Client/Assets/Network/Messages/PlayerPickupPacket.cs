
using System;
using System.Reflection;

public class PlayerPickupPacket : IMessage
{

    static PlayerPickupPacket()
    {
        SetTypes(MethodBase.GetCurrentMethod().DeclaringType,
            new Type[] {
                typeof(int), // ID 0
            }
        );
    }

    public PlayerPickupPacket() { }

    public PlayerPickupPacket(int id) {

        objects.Add(id);

    }

    public override void Read() { }

    public override void Write() {

        Client.instance.Send(this);

    }

}
