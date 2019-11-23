package me.wattguy.lotso.utils;

import me.wattguy.lotso.HotMessage;
import me.wattguy.lotso.Main;
import me.wattguy.lotso.enums.*;
import me.wattguy.lotso.objects.*;
import me.wattguy.lotso.packets.Packet;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;

public class Utils {

    public static List<ItemType> items;
    public static Random rand;

    static {

        items = new ArrayList<>();
        rand = new Random();

        ItemType[] values = ItemType.values();

        List<Integer> list = new ArrayList<>();

        int i = 0;
        while(i < values.length){

            list.add(i);
            i++;

        }

        while(list.size() > 0){

            int index = getRandom(0, list.size() - 1);
            items.add(values[list.get(index)]);
            list.remove(index);

        }

    }

    public static org.dyn4j.geometry.Vector2 degreesToVector2(float degrees){

        float radians = (float) Math.toRadians(degrees);
        return new org.dyn4j.geometry.Vector2(Math.cos(radians), Math.sin(radians));

    }

    public static org.dyn4j.geometry.Vector2 multiply(org.dyn4j.geometry.Vector2 v, float m){

        v.x *= m;
        v.y *= m;
        return v;

    }

    public static Packet packetInstance(Class<? extends Packet> c, Player p){

        try {
            Packet pt = c.getConstructor().newInstance();
            pt.setChannel(p.getHandler().ctx);
            pt.setPlayer(p);
            return pt;
        } catch (Exception e) {
            e.printStackTrace();
        }

        return null;

    }

    public static void writeAll(Class<? extends Packet> c, Object... objects){

        for(Player p : Main.players.values()){

            packetInstance(c, p).write(objects);

        }

    }

    public static Boolean isItSuitable(List<Class> types, Object... objects){

        try {
            if (objects.length < types.size() || objects.length > types.size()) return false;

            for(int i = 0; i < types.size(); i++){

                if(objects[i].getClass() != types.get(i)) return false;

            }

        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }

        return true;
    }

    public static String toString(Object... objects){
        StringBuilder sb = new StringBuilder();

        for(int i = 0; i < objects.length; i++){
            Object o = objects[i];

            sb.append(o);

            if (i != objects.length - 1) sb.append("|");

        }

        return sb.toString();
    }

    public static Boolean isItSuitable(List<Class> types, String s){

        try {
            String[] ss = s.split("\\|");

            if (ss.length < types.size() || ss.length > types.size()) return false;

        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }

        return true;
    }

    public static List<Object> fromString(List<Class> types, String s){
        List<Object> objects = new ArrayList<>();

        try {
            String[] ss = s.split("\\|");

            for(int i = 0; i < types.size(); i++){

                if (types.get(i) == String.class) objects.add(ss[i]);
                else if (types.get(i) == Integer.class){

                    try{

                        objects.add(Integer.parseInt(ss[i]));

                    }catch(Exception ignored){return null;}

                }else if (types.get(i) == Float.class){

                    try{

                        objects.add(Float.parseFloat(ss[i]));

                    }catch(Exception ignored){return null;}

                }else if (types.get(i) == Byte.class){

                    try{

                        objects.add(Byte.parseByte(ss[i]));

                    }catch(Exception ignored){return null;}

                }else if (types.get(i) == Gun.class){

                    try{

                        Gun g = Gun.fromString(ss[i]);
                        if (g != null) objects.add(g);

                    }catch(Exception ignored){return null;}

                }else if (types.get(i) == DestType.class){

                    try{

                        DestType dt = DestType.fromString(ss[i]);
                        if (dt != null) objects.add(dt);

                    }catch(Exception ignored){return null;}

                }else if (types.get(i) == IndestType.class){

                    try{

                        IndestType it = IndestType.fromString(ss[i]);
                        if (it != null) objects.add(it);

                    }catch(Exception ignored){return null;}

                }else if (types.get(i) == ItemType.class){

                    try{

                        ItemType it = ItemType.fromString(ss[i]);
                        if (it != null) objects.add(it);

                    }catch(Exception ignored){return null;}

                }else if (types.get(i) == ObjectType.class){

                    try{

                        ObjectType ot = ObjectType.fromString(ss[i]);
                        if (ot != null) objects.add(ot);

                    }catch(Exception ignored){return null;}

                }

            }

        } catch (Exception e) {
            e.printStackTrace();
            return null;
        }

        return objects;

    }

    public static Packet messageToPacket(HotMessage.Packet o, Player p){
        if(!Main.packets.containsKey(o.getId())) return null;

        return packetInstance(Main.packets.get(o.getId()), p);

    }

    public static String getType(GameObject go, ObjectType ot){

        switch(ot){

            case DESTRUCTIBLE:
                return ((Destructible) go).getType().toString();

            case INDESTRUCTIBLE:
                return ((Indestructible) go).getType().toString();

            case ITEM:
                return ((Item) go).getType().toString();

            default:
                return "";
        }

    }

    public static Double distance(Vector2 v1, Vector2 v2){

        return Math.sqrt(Math.pow(v2.x - v1.x, 2) + Math.pow(v2.y - v1.y, 2));

    }

    public static int getRandom(int min, int max) {
        return rand.nextInt((max - min) + 1) + min;
    }

    public static ItemType getRandomItem(){
        return items.get(getRandom(0, items.size() - 1));
    }

}
