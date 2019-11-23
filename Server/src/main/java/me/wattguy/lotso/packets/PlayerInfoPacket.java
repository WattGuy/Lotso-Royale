package me.wattguy.lotso.packets;

import me.wattguy.lotso.HotMessage;
import me.wattguy.lotso.enums.Gun;
import me.wattguy.lotso.utils.Utils;

import java.lang.invoke.MethodHandles;

public class PlayerInfoPacket extends Packet {

    static {

        setTypes(MethodHandles.lookup().lookupClass(),
                Integer.class, // ID
                Gun.class, // Gun
                Integer.class, // Helmet
                Integer.class, // Health
                Integer.class // Bullets
        );

    }

    public PlayerInfoPacket() {
        super(Type.PLAYER_INFO);
    }

    @Override
    public void write(Object... objects) {
        if (!Utils.isItSuitable(getTypes(), objects)) return;

        getChannel().writeAndFlush(HotMessage.Packet.newBuilder().setId(getType().getTag()).setMsg(Utils.toString(objects)).build());

    }

    @Override
    public void read(String in) { }

}
