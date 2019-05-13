using Google.Protobuf;

public class DestructibleDespawnMessage : IMessage
{

    public ushort GetTag() { return Tags.DESTRUCTIBLE_DESPAWN; }

    public ushort id;

    public DestructibleDespawnMessage(ushort id)
    {

        this.id = id;

    }

    public DestructibleDespawnMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public DestructibleDespawnMessage(string s)
    {
        initialize(s);
    }

    private void initialize(string s)
    {
        string[] ss = s.Split(';');

        this.id = ushort.Parse(ss[0]);

    }

    public ByteString ToByteString()
    {

        return ByteString.CopyFromUtf8(this.ToString());

    }

    override
    public string ToString()
    {

        return id.ToString();

    }

}
