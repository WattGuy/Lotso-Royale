using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ItemDespawnMessage : IMessage
{

    public ushort GetTag() { return Tags.ITEM_DESPAWN; }

    public ushort id;

    public ItemDespawnMessage(ushort id)
    {

        this.id = id;

    }

    public ItemDespawnMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public ItemDespawnMessage(string s)
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
