using Google.Protobuf;

public class BulletDestructibleHitMessage : IMessage
{

    public ushort GetTag() { return Tags.BULLET_DESTRUCTIBLE_HIT; }

    public ushort id;
    public ushort dest;

    public BulletDestructibleHitMessage(ushort id, ushort dest)
    {

        this.id = id;
        this.dest = dest;

    }

    public BulletDestructibleHitMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public BulletDestructibleHitMessage(string s)
    {
        initialize(s);
    }

    private void initialize(string s)
    {
        string[] ss = s.Split(';');

        this.id = ushort.Parse(ss[0]);
        this.dest = ushort.Parse(ss[1]);

    }

    public ByteString ToByteString()
    {

        return ByteString.CopyFromUtf8(this.ToString());

    }

    override
    public string ToString()
    {

        return id + ";" + dest;

    }

}
