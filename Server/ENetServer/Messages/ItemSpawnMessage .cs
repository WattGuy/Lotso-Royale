using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ItemSpawnMessage : IMessage
{

    public ushort GetTag() { return Tags.ITEM_SPAWN; }

    public ushort id;
    public Vector2 pos;
    public Item.Type t;

    public ItemSpawnMessage(ushort id, Vector2 pos, Item.Type t)
    {

        this.id = id;
        this.pos = pos;
        this.t = t;

    }

    public ItemSpawnMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public ItemSpawnMessage(string s)
    {
        initialize(s);
    }

    private void initialize(string s)
    {
        string[] ss = s.Split(';');

        this.id = ushort.Parse(ss[0]);
        this.pos = new Vector2(float.Parse(ss[1]), float.Parse(ss[2]));
        this.t = Item.FromString(ss[3]);

    }

    public ByteString ToByteString()
    {

        return ByteString.CopyFromUtf8(this.ToString());

    }

    override
    public string ToString()
    {

        return id + ";" + pos.x + ";" + pos.y + ";" + t.ToString();

    }

}
