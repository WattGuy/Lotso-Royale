using ENet;
using System;

public class Player {

    public ushort id;
    public Peer peer;

    public Vector2 pos;
    public Rotation rotation = new Rotation(0f, 0f);

    public Vector2 npos = null;
    public Rotation nrotation = null;

    public Gun gun = Gun.ARMS;
    public ushort health = 100;
    public ushort bullets = 0;
    public ushort helmet = 0;

    public Player(ushort id, Peer peer, Vector2 pos) {

        this.id = id;
        this.peer = peer;

        this.pos = pos;

    }

    public void Death() {

        if (gun != Gun.ARMS)
            Server.map.addItem(Item.FromString(gun.ToString()), pos);
        else
            Server.map.addItem(Item.types[new Random().Next(Item.types.Count)], pos);

        PlayerDespawnMessage pdm = new PlayerDespawnMessage(id);

        foreach (Player p in Server.players.Values) {

            Server.Send(p.peer, pdm, 0);

        }

    }

}
