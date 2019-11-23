package me.wattguy.lotso.packets;

import me.wattguy.lotso.HotMessage;
import me.wattguy.lotso.Main;
import me.wattguy.lotso.utils.Utils;

import java.lang.invoke.MethodHandles;

public class PingPacket extends Packet {

    static {

        setTypes(MethodHandles.lookup().lookupClass(), Byte.class);

    }

    public PingPacket() {
        super(Type.PING);
    }

    @Override
    public void write(Object... objects) {
        if (!Utils.isItSuitable(getTypes(), objects)) return;

        getChannel().writeAndFlush(HotMessage.Packet.newBuilder().setId(getType().getTag()).setMsg(Utils.toString(objects)).build());

    }

    @Override
    public void read(String in) {

        if (getPlayer().getDifference() >= Main.PING_TIME){

            getPlayer().setTime(System.currentTimeMillis());

            write(Byte.valueOf("1"));

        }

    }

}
