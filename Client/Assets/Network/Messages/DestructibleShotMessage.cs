using Google.Protobuf;

public class DestructibleShotMessage : IMessage
{

    public ushort GetTag() { return Tags.DESTRUCTIBLE_SHOT; }

    public ushort id;

    public DestructibleShotMessage(ushort id)
    {

        this.id = id;

    }

    public DestructibleShotMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public DestructibleShotMessage(string s)
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
