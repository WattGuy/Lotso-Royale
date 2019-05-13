using Google.Protobuf;

public class PlayerDespawnMessage : IMessage
{

    public ushort GetTag() { return Tags.PLAYER_DESPAWN; }

    public ushort id;

    public PlayerDespawnMessage(ushort id)
    {

        this.id = id;

    }

    public PlayerDespawnMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public PlayerDespawnMessage(string s)
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
