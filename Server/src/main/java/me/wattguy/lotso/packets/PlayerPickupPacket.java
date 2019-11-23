package me.wattguy.lotso.packets;

import me.wattguy.lotso.Main;
import me.wattguy.lotso.enums.ObjectType;
import me.wattguy.lotso.objects.GameObject;
import me.wattguy.lotso.objects.Item;
import me.wattguy.lotso.utils.Utils;

import java.lang.invoke.MethodHandles;
import java.util.List;

public class PlayerPickupPacket extends Packet {

    static {

        setTypes(MethodHandles.lookup().lookupClass(),
                Integer.class // ID
        );

    }

    public PlayerPickupPacket() {
        super(Type.PLAYER_PICKUP);
    }

    @Override
    public void write(Object... objects) { }

    @Override
    public void read(String in) {
        if (!Utils.isItSuitable(getTypes(), in)) return;
        List<Object> objects = Utils.fromString(getTypes(), in);

        if (!Main.map.objects.containsKey(objects.get(0))) return;

        GameObject go = Main.map.objects.get(objects.get(0));
        if (!(go instanceof Item)) return;

        if (!getPlayer().pickupItem((Item) go)) return;

        Utils.writeAll(ObjectDespawnPacket.class, go.getId(), ObjectType.ITEM);
        Main.map.objects.remove(go.getId());

    }

}
