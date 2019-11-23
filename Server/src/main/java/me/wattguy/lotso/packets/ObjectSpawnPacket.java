package me.wattguy.lotso.packets;

import me.wattguy.lotso.HotMessage;
import me.wattguy.lotso.enums.ObjectType;
import me.wattguy.lotso.utils.Utils;

import java.lang.invoke.MethodHandles;

public class ObjectSpawnPacket extends Packet {

    static {

        setTypes(MethodHandles.lookup().lookupClass(),
                Integer.class, // ID
                ObjectType.class, // Type of object
                String.class, // DestType | IndestType | ItemType
                Float.class, // X - Position
                Float.class, // Y - Position
                Integer.class // Health for destructible object
        );

    }

    public ObjectSpawnPacket() {
        super(Type.OBJECT_SPAWN);
    }

    @Override
    public void write(Object... objects) {
        if (!Utils.isItSuitable(getTypes(), objects)) return;

        getChannel().writeAndFlush(HotMessage.Packet.newBuilder().setId(getType().getTag()).setMsg(Utils.toString(objects)).build());

    }

    @Override
    public void read(String in) { }

}
