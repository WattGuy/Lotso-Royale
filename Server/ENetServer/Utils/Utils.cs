using System;

public enum Gun {
    M92, AK47, ARMS
}

public enum ObjectType {
    ITEM, DESTRUCTIBLE, INDESTRUCTIBLE
}

public class MapObject {

    public string obj = null;
    public float? x = null;
    public float? y = null;
    public string type = null;

}

public class SpawnPoint
{

    public float? x = null;
    public float? y = null;

}

public class Utils
{

    public static double Distance(Vector2 a, Vector2 b) {

        return Math.Sqrt(Math.Pow(b.x - a.x, 2) + Math.Pow(b.y - a.y, 2));

    }

    public static ushort getDamage(Gun g) {

        switch (g) {

            case Gun.AK47:
                return 7;

            case Gun.ARMS:
                return 8;

            case Gun.M92:
                return 4;

        }

        return 0;
    }

    public static void addMapObject(MapObject mobj) {
        string obj = mobj.obj.ToLower();

        if (obj == "item")
        {

            ushort id = Server.ider.getId();
            Server.map.objects.Add(id, new Item(id, Item.FromString(mobj.type.ToUpper()), new Vector2((float)mobj.x, (float)mobj.y)));

        }
        else if (obj == "destructible") {

            ushort id = Server.ider.getId();
            Server.map.objects.Add(id, new Destructible(id, Destructible.FromString(mobj.type.ToUpper()), new Vector2((float)mobj.x, (float)mobj.y)));

        }
        else if (obj == "indestructible")
        {

            ushort id = Server.ider.getId();
            Server.map.objects.Add(id, new Indestructible(id, Indestructible.FromString(mobj.type.ToUpper()), new Vector2((float)mobj.x, (float)mobj.y)));

        }

    }

    public static void addSpawnPoint(SpawnPoint sp)
    {

        Server.points.Add(new Vector2((float) sp.x, (float) sp.y));

    }

    public static Gun FromString(string s)
    {

        try
        {

            return (Gun)Enum.Parse(typeof(Gun), s);

        }
        catch (Exception e) { }

        return Gun.ARMS;

    }

}
