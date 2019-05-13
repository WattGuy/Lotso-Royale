using Google.Protobuf;
using UnityEngine;

public class PlayerSpawnMessage : IMessage
{

    public ushort GetTag() { return Tags.PLAYER_SPAWN; }

    public ushort id;
    public Vector2 pos;
    public float z;
    public float w;
    public Gun gun;
    public ushort helmet;

    public PlayerSpawnMessage(ushort id, Vector2 pos, float z, float w, Gun gun, ushort helmet)
    {

        this.id = id;
        this.pos = pos;
        this.z = z;
        this.w = w;
        this.gun = gun;
        this.helmet = helmet;

    }

    public PlayerSpawnMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public PlayerSpawnMessage(string s)
    {
        initialize(s);
    }

    private void initialize(string s)
    {
        string[] ss = s.Split(';');

        this.id = ushort.Parse(ss[0]);
        this.pos = new Vector2(float.Parse(ss[1]), float.Parse(ss[2]));
        this.z = float.Parse(ss[3]);
        this.w = float.Parse(ss[4]);
        this.gun = TypeUtils.getGun(ss[5]);
        this.helmet = ushort.Parse(ss[6]);

    }

    public ByteString ToByteString()
    {

        return ByteString.CopyFromUtf8(this.ToString());

    }

    override
    public string ToString()
    {

        return id + ";" + pos.x + ";" + pos.y + ";" + z + ";" + w + ";" + gun.ToString() + ";" + helmet;

    }

}
