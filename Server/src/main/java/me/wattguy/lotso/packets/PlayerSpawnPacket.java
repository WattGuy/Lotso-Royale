package me.wattguy.lotso.packets;

import me.wattguy.lotso.HotMessage;
import me.wattguy.lotso.utils.Utils;
import me.wattguy.lotso.enums.Gun;

import java.lang.invoke.MethodHandles;

public class PlayerSpawnPacket extends Packet {

    static {

        setTypes(MethodHandles.lookup().lookupClass(),
                Integer.class, // ID
                Float.class, // X - Position
                Float.class, // Y - Position
                Float.class, // Z - Quaternion
                Float.class, // W - Quaternion
                Gun.class, // Gun
                Integer.class // Helmet
        );

    }

    public PlayerSpawnPacket() {
        super(Type.PLAYER_SPAWN);
    }

    @Override
    public void write(Object... objects) {
        if (!Utils.isItSuitable(getTypes(), objects)) return;

        getChannel().writeAndFlush(HotMessage.Packet.newBuilder().setId(getType().getTag()).setMsg(Utils.toString(objects)).build());

    }

    @Override
    public void read(String in) { }

}
