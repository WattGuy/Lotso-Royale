using ENet;
using System;
using System.Collections.Generic;
using System.Linq;

public class Item
{

    public static List<Type> types = Enum.GetValues(typeof(Type)).Cast<Type>().ToList();
    public ushort id;
    public Type t;
    public Vector2 pos;

    public Item(ushort id, Type t, Vector2 pos) {

        this.id = id;
        this.t = t;
        this.pos = pos;

    }

    public void AddTo(Player p) {

        switch (t) {

            case Type.AK47:
            case Type.M92:
                p.gun = Utils.FromString(t.ToString());
                p.bullets = (ushort) t;
                break;

            case Type.BULLETS:
                p.bullets += 20;
                break;

            case Type.HELMET1:
            case Type.HELMET2:
                p.helmet = ushort.Parse(t.ToString().Replace("HELMET", ""));
                break;

            case Type.MEDKIT:
            case Type.BANDAGES:
                if (p.health + (ushort) t >= 100) p.health = 100;
                else p.health += (ushort) t;
                break;

        }

        PlayerInfoMessage pim = new PlayerInfoMessage(p.id, p.gun, p.bullets, p.health, p.helmet);

        foreach (Player o in Server.players.Values) {

            Server.Send(o.peer, pim, 0, PacketFlags.Reliable);

        }

    }

    public enum Type {
        M92 = 40, AK47 = 20, BULLETS, HELMET1, HELMET2, BANDAGES = 25, MEDKIT = 80
    }

    public static Type FromString(string s)
    {

        try
        {

            return (Type) Enum.Parse(typeof(Type), s);

        }
        catch (Exception e) { }

        return Type.BANDAGES;

    }

}
