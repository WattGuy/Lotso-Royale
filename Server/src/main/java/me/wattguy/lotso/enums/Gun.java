package me.wattguy.lotso.enums;

public enum Gun {
    M92, AK47, ARMS;

    public static Gun fromString(String s){

        try { return Gun.valueOf(s); }
        catch(Exception ignored) { return null; }

    }

}
