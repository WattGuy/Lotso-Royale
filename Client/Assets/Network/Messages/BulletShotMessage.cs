using Google.Protobuf;
using UnityEngine;

public class BulletShotMessage : IMessage
{

    public ushort GetTag() { return Tags.BULLET_SHOT; }

    public ushort id;
    public ushort bullet;
    public Vector2 pos;
    public float z;
    public float w;

    public BulletShotMessage(ushort id, ushort bullet, Vector2 pos, float z, float w)
    {

        this.id = id;
        this.bullet = bullet;
        this.pos = pos;
        this.z = z;
        this.w = w;

    }

    public BulletShotMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public BulletShotMessage(string s)
    {
        initialize(s);
    }

    private void initialize(string s)
    {
        string[] ss = s.Split(';');

        this.id = ushort.Parse(ss[0]);
        this.bullet = ushort.Parse(ss[1]);
        this.pos = new Vector2(float.Parse(ss[2]), float.Parse(ss[3]));
        this.z = float.Parse(ss[4]);
        this.w = float.Parse(ss[5]);

    }

    public ByteString ToByteString()
    {

        return ByteString.CopyFromUtf8(this.ToString());

    }

    override
    public string ToString()
    {

        return id + ";" + bullet + ";" + pos.x + ";" + pos.y + ";" + z + ";" + w;

    }

}