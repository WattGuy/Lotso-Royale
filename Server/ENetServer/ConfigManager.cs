using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ConfigManager
{

    public ushort minplayers = 2;
    public ushort minseconds = 30;

    public ushort boostplayers = 5;
    public ushort boostseconds = 10;

    public void Initialize()
    {
        string path = AppDomain.CurrentDomain.BaseDirectory;
        string configpath = path + "config.json";

        Config(configpath);

        string mappath = path + "map.json";

        Map(mappath);

        string pointspath = path + "points.json";

        Points(pointspath);

    }

    private void Config(string configpath) {

        if (File.Exists(configpath))
        {

            JObject o = JObject.Parse(load(configpath));

            if (o.ContainsKey("ip"))
            {

                Server.ip = (string)o["ip"];

            }

            if (o.ContainsKey("port"))
            {

                try
                {

                    Server.port = ushort.Parse((string)o["port"]);

                }
                catch { }

            }

            if (o.ContainsKey("players"))
            {

                try
                {

                    Server.slots = ushort.Parse((string)o["players"]);

                }
                catch { }

            }

            if (o.ContainsKey("min") && o["min"] is JObject) {
                JObject min = (JObject) o["min"];

                if (min.ContainsKey("players")) {

                    try
                    {

                        minplayers = ushort.Parse((string) min["players"]);

                    }
                    catch { }

                }

                if (min.ContainsKey("seconds"))
                {

                    try
                    {

                        minseconds = ushort.Parse((string)min["seconds"]);

                    }
                    catch { }

                }

            }

            if (o.ContainsKey("boost") && o["boost"] is JObject)
            {
                JObject boost = (JObject)o["boost"];

                if (boost.ContainsKey("boost"))
                {

                    try
                    {

                        boostplayers = ushort.Parse((string) boost["players"]);

                    }
                    catch { }

                }

                if (boost.ContainsKey("seconds"))
                {

                    try
                    {

                        boostseconds = ushort.Parse((string) boost["seconds"]);

                    }
                    catch { }

                }

            }

        }
        else
        {

            save(
                configpath,
                new JObject(
                    new JProperty("ip", Server.ip),
                    new JProperty("port", Server.port),
                    new JProperty("players", 15),
                    new JProperty("min", new JObject(
                            new JProperty("players", 2),
                            new JProperty("seconds", 30)
                        )),
                    new JProperty("boost", new JObject(
                            new JProperty("players", 5),
                            new JProperty("seconds", 10)
                        ))
                ).ToString()
            );

        }

    }

    private void Map(string mappath) {

        if (File.Exists(mappath))
        {
            JArray array = JArray.Parse(load(mappath));

            foreach (JObject obj in array.Children<JObject>())
            {
                MapObject mobj = new MapObject();

                foreach (JProperty p in obj.Properties())
                {
                    string name = p.Name.ToLower();

                    if (p.Name == "object") {

                        mobj.obj = p.Value.ToString();

                    } else if (p.Name == "x") {

                        try
                        {
                            mobj.x = float.Parse(p.Value.ToString());
                        }
                        catch { }

                    }
                    else if (p.Name == "y")
                    {

                        try
                        {
                            mobj.y = float.Parse(p.Value.ToString());
                        }
                        catch { }

                    }
                    else if (p.Name == "type")
                    {

                        mobj.type = p.Value.ToString();

                    }

                }

                if (mobj.obj != null && mobj.x != null && mobj.y != null && mobj.type != null) {

                    Utils.addMapObject(mobj);

                }

            }

        }

    }

    private void Points(string pointspath) {

        if (File.Exists(pointspath))
        {
            JArray array = JArray.Parse(load(pointspath));

            foreach (JObject obj in array.Children<JObject>())
            {
                SpawnPoint sp = new SpawnPoint();

                foreach (JProperty p in obj.Properties())
                {
                    string name = p.Name.ToLower();

                    if (p.Name == "x")
                    {

                        try
                        {
                            sp.x = float.Parse(p.Value.ToString());
                        }
                        catch { }

                    }
                    else if (p.Name == "y")
                    {

                        try
                        {
                            sp.y = float.Parse(p.Value.ToString());
                        }
                        catch { }

                    }

                }

                if (sp.x != null && sp.y != null)
                {

                    Utils.addSpawnPoint(sp);

                }

            }

        }

    }

    public static string load(string path)
    {
        StringBuilder output = new StringBuilder();

        foreach (string line in File.ReadAllLines(path))
        {

            output.Append(line);

        }

        return output.ToString();

    }

    public static void save(string path, string value)
    {

        using (StreamWriter sw = File.CreateText(path))
        {

            sw.WriteLine(value);

        }
    }

}
