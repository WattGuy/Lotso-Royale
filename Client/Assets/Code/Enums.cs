
using System;

public enum Gun {
    M92, AK47, ARMS

}

public enum Helmet {
    NONE = 0, HELMET1 = 1, HELMET2 = 2

}

public enum Item
{
    M92, AK47, BULLETS, HELMET1, HELMET2, BANDAGES, MEDKIT

}

public enum DestructibleType {
    STONE, CRATE,
}

public enum IndestructibleType {
    BUSH
}

public enum ObjectType {
    DESTRUCTIBLE, INDESTRUCTIBLE, ITEM, PLAYER, BULLET, NONE
}

public class TypeUtils {

    public static string getName(string s) {
        char[] cs = s.ToLower().ToCharArray();
        cs[0] = Char.ToUpper(cs[0]);

        return new string(cs);

    }

    public static DestructibleType getDestructible(string s)
    {
        try
        {

            return (DestructibleType) Enum.Parse(typeof(DestructibleType), s);

        }
        catch (Exception e) { }

        return DestructibleType.STONE;
    }

    public static IndestructibleType getIndestructible(string s)
    {
        try
        {

            return (IndestructibleType)Enum.Parse(typeof(IndestructibleType), s);

        }
        catch (Exception e) { }

        return IndestructibleType.BUSH;
    }

    public static Gun getGun(string s)
    {
        try
        {

            return (Gun)Enum.Parse(typeof(Gun), s);

        }
        catch (Exception e) { }

        return Gun.ARMS;
    }

    public static Item getItem(string s)
    {
        try
        {

            return (Item)Enum.Parse(typeof(Item), s);

        }
        catch (Exception e) { }

        return Item.BANDAGES;
    }

}

