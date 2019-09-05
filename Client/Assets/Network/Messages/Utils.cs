using System;
using System.Collections.Generic;
using System.Linq;

public class Utils
{

    public static Dictionary<int, Type> types = new Dictionary<int, Type>();
    public static Dictionary<Type, int> ids = new Dictionary<Type, int>();

    static Utils() {

        Add(Tags.PING, typeof(PingPacket));
        Add(Tags.PLAYER_SPAWN, typeof(PlayerSpawnPacket));
        Add(Tags.INFO, typeof(ClientInfoPacket));
        Add(Tags.OBJECT_SPAWN, typeof(ObjectSpawnPacket));
        Add(Tags.PLAYER_MOVE, typeof(PlayerMovePacket));
        Add(Tags.PLAYER_SHOT, typeof(PlayerShotPacket));
        Add(Tags.OBJECT_DESPAWN, typeof(ObjectDespawnPacket));
        Add(Tags.PLAYER_INFO, typeof(PlayerInfoPacket));
        Add(Tags.CIRCLE_INFO, typeof(CircleInfoPacket));
        Add(Tags.PLAYER_PICKUP, typeof(PlayerPickupPacket));
        Add(Tags.OBJECT_INFO, typeof(ObjectInfoPacket));

    }

    public static IMessage getPacket(Packet p) {
        if (!types.ContainsKey(p.Id)) return null;

        IMessage msg = (IMessage) Activator.CreateInstance(types[p.Id]);
        msg.Initialize(p.Msg);

        return msg;

    }

    public static int? GetID(Type t) {

        if (!ids.ContainsKey(t)) return null;
        else return ids[t];

    }

    public static void Add(int id, Type t) {

        types.Add(id, t);
        ids.Add(t, id);

    }

}