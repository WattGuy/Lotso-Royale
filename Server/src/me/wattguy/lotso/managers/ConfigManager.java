package me.wattguy.lotso.managers;

import me.wattguy.lotso.Main;
import me.wattguy.lotso.enums.DestType;
import me.wattguy.lotso.enums.IndestType;
import me.wattguy.lotso.enums.ItemType;
import me.wattguy.lotso.enums.ObjectType;
import me.wattguy.lotso.objects.Destructible;
import me.wattguy.lotso.objects.Indestructible;
import me.wattguy.lotso.objects.Item;
import me.wattguy.lotso.utils.Vector2;
import org.json.JSONArray;
import org.json.JSONObject;
import org.json.JSONTokener;

import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;

public class ConfigManager {

    private String path = System.getProperty("user.dir") + "\\";

    public ConfigManager(){

        config(path + "config.json");

    }

    private void config(String path){

        try {
            File f = new File(path);

            if (f.exists()) {

                JSONObject o = new JSONObject(new JSONTokener(new FileReader(path)));

                if (o.has("port")) Main.TCP_PORT = ((Integer) o.get("port")).shortValue();

                if (o.has("timeout")) Main.TIMEOUT = (Integer) o.get("timeout");

                if (o.has("debug")) Main.DEBUG = (Boolean) o.get("debug");

            }else{

                JSONObject o = new JSONObject();
                o.put("port", Main.TCP_PORT);
                o.put("timeout", Main.TIMEOUT);
                o.put("debug", Main.DEBUG);

                try (FileWriter file = new FileWriter(path)) {

                    file.write(o.toString(2));
                    file.flush();

                }

            }

        }catch(Exception ex){
            ex.printStackTrace();
        }

    }

    public void map(){
        String path = this.path + "map.json";

        try {
            File f = new File(path);
            if (!f.exists()) return;

            JSONArray array = new JSONArray(new JSONTokener(new FileReader(path)));

            for(Object obj : array){
                if(!(obj instanceof JSONObject)) continue;

                CacheObject co = new CacheObject();
                JSONObject o = (JSONObject) obj;

                if (o.has("object")) co.object = o.getString("object");

                if (o.has("type")) co.type = o.getString("type");

                if (o.has("x")) co.x = o.getFloat("x");

                if (o.has("y")) co.y = o.getFloat("y");

                co.export();
            }

        }catch(Exception ex){
            ex.printStackTrace();
        }

    }

    public static class CacheObject {

        public String object = null;
        public String type = null;
        public Float x = null;
        public Float y = null;

        private Boolean thatsAll(){
            return object != null && type != null && x != null && y != null;
        }

        public void export(){
            if (!thatsAll()) return;
            ObjectType ot = ObjectType.fromString(object.toUpperCase());
            if (ot == null) return;

            Integer id = Main.ider.next();

            if(ot == ObjectType.ITEM){

                ItemType it = ItemType.fromString(type.toUpperCase());
                if (it == null) return;

                Main.map.objects.put(id, new Item(id, new Vector2(x, y), it));

            }else if (ot == ObjectType.DESTRUCTIBLE){

                DestType dt = DestType.fromString(type.toUpperCase());
                if (dt == null) return;

                Main.map.objects.put(id, new Destructible(id, new Vector2(x, y), dt));

            }else if (ot == ObjectType.INDESTRUCTIBLE){

                IndestType it = IndestType.fromString(type.toUpperCase());
                if (it == null) return;

                Main.map.objects.put(id, new Indestructible(id, new Vector2(x, y), it));

            }

        }

    }

}
