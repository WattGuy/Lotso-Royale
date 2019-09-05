package me.wattguy.lotso.enums;

public enum DestType {
    STONE(11), CRATE(4);

    private Integer health;

    DestType(Integer health){

        this.health = health;

    }

    public Integer getHealth(){
        return health;
    }

    public static DestType fromString(String s){

        try { return DestType.valueOf(s); }
        catch(Exception ignored) { return null; }

    }

}
