using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Destructible
{

    public ushort id;
    public Type t;
    public ushort shots = 0;
    public Vector2 pos;

    public Destructible(ushort id, Type t, Vector2 pos) {

        this.id = id;
        this.t = t;
        this.pos = pos;

    }

    public void Check() {
        if (t != Type.CRATE) return;

        Server.map.addItem(Item.types[new Random().Next(Item.types.Count)], pos);

    }

    public enum Type
    {
        STONE = 11, CRATE = 4
    }

    public static Type FromString(string s) {

        try
        {

            return (Type) Enum.Parse(typeof(Type), s);

        }
        catch (Exception e) { }

        return Type.STONE;

    }

}
