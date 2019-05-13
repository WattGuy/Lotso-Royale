using Google.Protobuf;
using UnityEngine;

public class CircleSpawnMessage : IMessage
{

    public ushort GetTag() { return Tags.CIRCLE_SPAWN; }

    public ushort id;
    public Vector2 pos;
    public float scale;

    public CircleSpawnMessage(ushort id, Vector2 pos, float scale)
    {

        this.id = id;
        this.pos = pos;
        this.scale = scale;

    }

    public CircleSpawnMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public CircleSpawnMessage(string s)
    {
        initialize(s);
    }

    private void initialize(string s)
    {
        string[] ss = s.Split(';');

        this.id = ushort.Parse(ss[0]);
        this.pos = new Vector2(float.Parse(ss[1]), float.Parse(ss[2]));
        this.scale = float.Parse(ss[3]);

    }

    public ByteString ToByteString()
    {

        return ByteString.CopyFromUtf8(this.ToString());

    }

    override
    public string ToString()
    {

        return id + ";" + pos.x + ";" + pos.y + ";" + scale;

    }

}
