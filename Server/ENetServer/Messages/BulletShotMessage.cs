using Google.Protobuf;

public class BulletShotMessage : IMessage
{

    public ushort GetTag() { return Tags.BULLET_SHOT; }

    public ushort id;
    public ushort bullet;
    public Vector2 pos;
    public Rotation rotation;

    public BulletShotMessage(ushort id, ushort bullet, Vector2 pos, Rotation rotation)
    {

        this.id = id;
        this.bullet = bullet;
        this.pos = pos;
        this.rotation = rotation;

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
        this.rotation = new Rotation(float.Parse(ss[4]), float.Parse(ss[5]));

    }

    public ByteString ToByteString()
    {

        return ByteString.CopyFromUtf8(this.ToString());

    }

    override
    public string ToString()
    {

        return id + ";" + bullet + ";" + pos.x + ";" + pos.y + ";" + rotation.z + ";" + rotation.w;

    }

}