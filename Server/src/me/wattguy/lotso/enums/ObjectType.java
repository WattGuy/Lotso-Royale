package me.wattguy.lotso.enums;

import me.wattguy.lotso.objects.*;

public enum ObjectType {
    DESTRUCTIBLE, INDESTRUCTIBLE, ITEM, PLAYER, BULLET, NONE;

    public static ObjectType fromString(String s){

        try { return ObjectType.valueOf(s); }
        catch(Exception ignored) { return null; }

    }

    public static ObjectType fromObject(GameObject go){

        if (go instanceof Destructible) return ObjectType.DESTRUCTIBLE;
        else if (go instanceof Indestructible) return ObjectType.INDESTRUCTIBLE;
        else if (go instanceof Item) return ObjectType.ITEM;
        else if (go instanceof Player) return ObjectType.PLAYER;
        else return ObjectType.NONE;

    }

}
