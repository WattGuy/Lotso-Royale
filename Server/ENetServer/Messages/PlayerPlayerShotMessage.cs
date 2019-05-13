using Google.Protobuf;

public class PlayerPlayerShotMessage : IMessage
{

    public ushort GetTag() { return Tags.PLAYER_PLAYER_SHOT; }

    public ushort id;
    public ushort enemy;

    public PlayerPlayerShotMessage(ushort id, ushort enemy)
    {

        this.id = id;
        this.enemy = enemy;

    }

    public PlayerPlayerShotMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public PlayerPlayerShotMessage(string s)
    {
        initialize(s);
    }

    private void initialize(string s)
    {
        string[] ss = s.Split(';');

        this.id = ushort.Parse(ss[0]);
        this.enemy = ushort.Parse(ss[1]);

    }

    public ByteString ToByteString()
    {

        return ByteString.CopyFromUtf8(this.ToString());

    }

    override
    public string ToString()
    {

        return id + ";" + enemy;

    }

}