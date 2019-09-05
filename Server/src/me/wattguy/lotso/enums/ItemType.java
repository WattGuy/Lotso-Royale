package me.wattguy.lotso.enums;

public enum ItemType {
    M92, AK47, BULLETS, HELMET1, HELMET2, BANDAGES, MEDKIT;

    public static ItemType fromString(String s){

        try { return ItemType.valueOf(s); }
        catch(Exception ignored) { return null; }

    }

}
