using Google.Protobuf;

public class ZoneAttackMessage : IMessage
{

    public ushort GetTag() { return Tags.ZONE_ATTACK; }

    public ushort id;
    public ushort damage;

    public ZoneAttackMessage(ushort id, ushort damage)
    {

        this.id = id;
        this.damage = damage;

    }

    public ZoneAttackMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public ZoneAttackMessage(string s)
    {
        initialize(s);
    }

    private void initialize(string s)
    {
        string[] ss = s.Split(';');

        this.id = ushort.Parse(ss[0]);
        this.damage = ushort.Parse(ss[1]);

    }

    public ByteString ToByteString()
    {

        return ByteString.CopyFromUtf8(this.ToString());

    }

    override
    public string ToString()
    {

        return id + ";" + damage;

    }

}
