package me.wattguy.lotso.packets;

import me.wattguy.lotso.HotMessage;
import me.wattguy.lotso.Main;
import me.wattguy.lotso.objects.Player;
import me.wattguy.lotso.utils.Rotation;
import me.wattguy.lotso.utils.Utils;
import me.wattguy.lotso.utils.Vector2;

import java.lang.invoke.MethodHandles;
import java.util.List;

public class PlayerMovePacket extends Packet {

    static {

        setTypes(MethodHandles.lookup().lookupClass(),
                Integer.class, // ID 0
                Float.class, // X - Position 1
                Float.class, // Y - Position 2
                Float.class, // Z - Quaternion 3
                Float.class // W - Quaternion 4
        );

    }

    public PlayerMovePacket() {
        super(Type.PLAYER_MOVE);
    }

    @Override
    public void write(Object... objects) {
        if (!Utils.isItSuitable(getTypes(), objects)) return;

        getChannel().writeAndFlush(HotMessage.Packet.newBuilder().setId(getType().getTag()).setMsg(Utils.toString(objects)).build());
    }

    @Override
    public void read(String in) {
        if (!Utils.isItSuitable(getTypes(), in)) return;
        List<Object> objects = Utils.fromString(getTypes(), in);

        if (!Main.players.containsKey(objects.get(0)) || objects.get(0) != getPlayer().getId()) return;

        Player p = getPlayer();
        p.setPosition(new Vector2((Float) objects.get(1), (Float) objects.get(2)));
        p.setRotation(new Rotation((Float) objects.get(3), (Float) objects.get(4)));

        for(Player o : Main.players.values()){
            if (o == p) continue;

            Utils.packetInstance(PlayerMovePacket.class, o).write(objects.toArray());

        }

    }

}
