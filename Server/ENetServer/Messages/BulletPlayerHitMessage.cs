using Google.Protobuf;

class BulletPlayerHitMessage : IMessage
{

    public ushort GetTag() { return Tags.BULLET_PLAYER_HIT; }

    public ushort id;
    public ushort player;

    public BulletPlayerHitMessage(ushort id, ushort player)
    {

        this.id = id;
        this.player = player;

    }

    public BulletPlayerHitMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public BulletPlayerHitMessage(string s)
    {
        initialize(s);
    }

    private void initialize(string s)
    {
        string[] ss = s.Split(';');

        this.id = ushort.Parse(ss[0]);
        this.player = ushort.Parse(ss[1]);

    }

    public ByteString ToByteString()
    {

        return ByteString.CopyFromUtf8(this.ToString());

    }

    override
    public string ToString()
    {

        return id + ";" + player;

    }

}
