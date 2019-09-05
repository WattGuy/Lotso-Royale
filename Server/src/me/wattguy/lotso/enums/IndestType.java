package me.wattguy.lotso.enums;

public enum IndestType {
    BUSH;

    public static IndestType fromString(String s){

        try { return IndestType.valueOf(s); }
        catch(Exception ignored) { return null; }

    }

}
