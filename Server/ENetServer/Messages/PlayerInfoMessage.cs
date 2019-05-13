using Google.Protobuf;

public class PlayerInfoMessage : IMessage
{

    public ushort GetTag() { return Tags.PLAYER_INFO; }

    public ushort id;
    public Gun gun;
    public ushort bullets;
    public ushort health;
    public ushort helmet;

    public PlayerInfoMessage(ushort id, Gun gun, ushort bullets, ushort health, ushort helmet)
    {

        this.id = id;
        this.gun = gun;
        this.bullets = bullets;
        this.health = health;
        this.helmet = helmet;

    }

    public PlayerInfoMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public PlayerInfoMessage(string s)
    {
        initialize(s);
    }

    private void initialize(string s)
    {
        string[] ss = s.Split(';');

        this.id = ushort.Parse(ss[0]);
        this.gun = Utils.FromString(ss[1]);
        this.bullets = ushort.Parse(ss[2]);
        this.health = ushort.Parse(ss[3]);
        this.helmet = ushort.Parse(ss[4]);

    }

    public ByteString ToByteString()
    {

        return ByteString.CopyFromUtf8(this.ToString());

    }

    override
    public string ToString()
    {

        return id + ";" + gun.ToString() + ";" + bullets + ";" + health + ";" + helmet;

    }

}
