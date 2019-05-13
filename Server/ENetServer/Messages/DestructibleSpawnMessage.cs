using Google.Protobuf;

public class DestructibleSpawnMessage : IMessage
{

    public ushort GetTag() { return Tags.DESTRUCTIBLE_SPAWN; }

    public ushort id;
    public Vector2 pos;
    public Destructible.Type t;
    public ushort shots;

    public DestructibleSpawnMessage(ushort id, Vector2 pos, Destructible.Type t, ushort shots)
    {

        this.id = id;
        this.pos = pos;
        this.t = t;
        this.shots = shots;

    }

    public DestructibleSpawnMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public DestructibleSpawnMessage(string s)
    {
        initialize(s);
    }

    private void initialize(string s)
    {
        string[] ss = s.Split(';');

        this.id = ushort.Parse(ss[0]);
        this.pos = new Vector2(float.Parse(ss[1]), float.Parse(ss[2]));
        this.t = Destructible.FromString(ss[3]);
        this.shots = ushort.Parse(ss[4]);

    }

    public ByteString ToByteString()
    {

        return ByteString.CopyFromUtf8(this.ToString());

    }

    override
    public string ToString()
    {

        return id + ";" + pos.x + ";" + pos.y + ";" + t.ToString() + ";" + shots;

    }

}
