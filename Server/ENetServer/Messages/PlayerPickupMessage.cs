using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerPickupMessage : IMessage
{

    public ushort GetTag() { return Tags.PLAYER_PICKUP; }

    public ushort id;

    public PlayerPickupMessage(ushort id)
    {

        this.id = id;

    }

    public PlayerPickupMessage(ByteString s)
    {
        initialize(s.ToStringUtf8());
    }

    public PlayerPickupMessage(string s)
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
