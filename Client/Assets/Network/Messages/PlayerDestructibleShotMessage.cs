using Google.Protobuf;
using UnityEngine;

public class PlayerDestructibleShotMessage : IMessage
{

    public ushort GetTag() { return Tags.PLAYER_DESTRUCTIBLE_SHOT; }

    public ushort id;
    public ushort dest;
    public bool b;

    public PlayerDestructibleShotMessage(ushort id, bool b)
    {

        this.b = b;
        this.id = id;

    }

    public PlayerDestructibleShotMessage(ushort id, ushort dest, bool b)
    {

        this.id = id;
        this.dest = dest;
        this.b = b;

    }

    public PlayerDestructibleShotMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public PlayerDestructibleShotMessage(string s)
    {
        initialize(s);
    }

    private void initialize(string s)
    {
        string[] ss = s.Split(';');

        this.id = ushort.Parse(ss[0]);
        this.dest = ushort.Parse(ss[1]);
        this.b = bool.Parse(ss[2]);

    }

    public ByteString ToByteString()
    {

        return ByteString.CopyFromUtf8(this.ToString());

    }

    override
    public string ToString()
    {

        return id + ";" + dest + ";" + b;

    }

}