using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MapManager
{

    public Dictionary<ushort, Object> objects = new Dictionary<ushort, Object>();

    public void OnConnect() {

        Server.Send(Server.netEvent.Peer, new Message() { Msg = ByteString.CopyFromUtf8("" + objects.Count), MsgType = Tags.OBJECTS_NUMBER }, Server.netEvent.ChannelID, ENet.PacketFlags.Reliable);

        foreach (object o in objects.Values) {

            if (o is Destructible dest)
            {
                DestructibleSpawnMessage dsm = new DestructibleSpawnMessage(dest.id, dest.pos, dest.t, dest.shots);

                Server.Send(Server.netEvent.Peer, dsm, Server.netEvent.ChannelID, ENet.PacketFlags.Reliable);

            }
            else if (o is Indestructible indest)
            {
                IndestructibleSpawnMessage ism = new IndestructibleSpawnMessage(indest.id, indest.pos, indest.t);

                Server.Send(Server.netEvent.Peer, ism, Server.netEvent.ChannelID, ENet.PacketFlags.Reliable);

            } else if (o is Item item) {
                ItemSpawnMessage ism = new ItemSpawnMessage(item.id, item.pos, item.t);

                Server.Send(Server.netEvent.Peer, ism, Server.netEvent.ChannelID, ENet.PacketFlags.Reliable);

            }

        }

    }

    public Item addItem(Item.Type t, Vector2 pos) {
        ushort i = Server.ider.getId();
        Item item = new Item(i, t, pos);

        objects.Add(i, item);

        ItemSpawnMessage ism = new ItemSpawnMessage(item.id, item.pos, item.t);
        foreach (Player p in Server.players.Values) {

            Server.Send(p.peer, ism, 0, ENet.PacketFlags.Reliable);

        }

        return item;
    }

    public void removeItem(ushort id) {
        objects.Remove(id);

        ItemDespawnMessage idm = new ItemDespawnMessage(id);
        foreach (Player p in Server.players.Values)
        {

            Server.Send(p.peer, idm, 0, ENet.PacketFlags.Reliable);

        }

    }

}
