using Google.Protobuf;
using UnityEngine;

public class IndestructibleSpawnMessage : IMessage
{

    public ushort GetTag() { return Tags.INDESTRUCTIBLE_SPAWN; }

    public ushort id;
    public Vector2 pos;
    public IndestructibleType t;

    public IndestructibleSpawnMessage(ushort id, Vector2 pos, IndestructibleType t)
    {

        this.id = id;
        this.pos = pos;
        this.t = t;

    }

    public IndestructibleSpawnMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public IndestructibleSpawnMessage(string s)
    {
        initialize(s);
    }

    private void initialize(string s)
    {
        string[] ss = s.Split(';');

        this.id = ushort.Parse(ss[0]);
        this.pos = new Vector2(float.Parse(ss[1]), float.Parse(ss[2]));
        this.t = TypeUtils.getIndestructible(ss[3]);

    }

    public ByteString ToByteString()
    {

        return ByteString.CopyFromUtf8(this.ToString());

    }

    override
    public string ToString()
    {

        return id + ";" + pos.x + ";" + pos.y + ";" + t.ToString();

    }

}
