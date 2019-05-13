using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Indestructible
{

    public ushort id;
    public Type t;
    public Vector2 pos;

    public Indestructible(ushort id, Type t, Vector2 pos)
    {

        this.id = id;
        this.t = t;
        this.pos = pos;

    }

    public enum Type
    {
        BUSH
    }

    public static Type FromString(string s)
    {

        try
        {

            return (Type) Enum.Parse(typeof(Type), s);

        }
        catch (Exception e) { }

        return Type.BUSH;
    }

}
