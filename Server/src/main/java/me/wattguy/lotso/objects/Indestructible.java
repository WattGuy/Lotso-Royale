package me.wattguy.lotso.objects;

import me.wattguy.lotso.enums.IndestType;
import me.wattguy.lotso.utils.Vector2;

public class Indestructible extends GameObject {

    private IndestType t;

    public Indestructible(Integer id, Vector2 position, IndestType t) {
        super(id, position);

        this.t = t;
    }

    public IndestType getType() {
        return t;
    }

}
