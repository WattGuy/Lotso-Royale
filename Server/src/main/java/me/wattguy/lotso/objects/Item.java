package me.wattguy.lotso.objects;

import me.wattguy.lotso.enums.CircleType;
import me.wattguy.lotso.enums.ItemType;
import me.wattguy.lotso.utils.Circle;
import me.wattguy.lotso.utils.Vector2;

public class Item extends GameObject {

    private ItemType t;
    private Circle circle;

    public Item(Integer id, Vector2 position, ItemType t) {
        super(id, position);

        this.t = t;
        this.circle = new Circle(position, .92f, CircleType.ITEM, null, false);
    }

    public ItemType getType() {
        return t;
    }

    public Circle getCircle() {
        return circle;
    }

}
