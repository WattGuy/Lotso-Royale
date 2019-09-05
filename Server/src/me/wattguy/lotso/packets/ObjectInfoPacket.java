package me.wattguy.lotso.packets;

import me.wattguy.lotso.HotMessage;
import me.wattguy.lotso.enums.ObjectType;
import me.wattguy.lotso.utils.Utils;

import java.lang.invoke.MethodHandles;

public class ObjectInfoPacket extends Packet {

    static {

        setTypes(MethodHandles.lookup().lookupClass(),
                Integer.class, // ID
                Integer.class // Health
        );

    }

    public ObjectInfoPacket() {
        super(Type.OBJECT_INFO);
    }

    @Override
    public void write(Object... objects) {
        if (!Utils.isItSuitable(getTypes(), objects)) return;

        getChannel().writeAndFlush(HotMessage.Packet.newBuilder().setId(getType().getTag()).setMsg(Utils.toString(objects)).build());

    }

    @Override
    public void read(String in) { }

}