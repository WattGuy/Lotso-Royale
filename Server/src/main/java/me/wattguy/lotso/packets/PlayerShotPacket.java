package me.wattguy.lotso.packets;

import me.wattguy.lotso.HotMessage;
import me.wattguy.lotso.Main;
import me.wattguy.lotso.enums.Gun;
import me.wattguy.lotso.enums.ObjectType;
import me.wattguy.lotso.objects.Bullet;
import me.wattguy.lotso.objects.Destructible;
import me.wattguy.lotso.objects.Player;
import me.wattguy.lotso.utils.Utils;

import java.lang.invoke.MethodHandles;
import java.util.ArrayList;
import java.util.List;

public class PlayerShotPacket extends Packet {

    static {

        setTypes(MethodHandles.lookup().lookupClass(),
                Integer.class, // ID
                ObjectType.class, // Type of object
                Integer.class, // ID of object
                Float.class // Degrees
        );

    }

    public static List<ObjectType> ts = new ArrayList<ObjectType>(){{

        add(ObjectType.DESTRUCTIBLE);
        add(ObjectType.PLAYER);
        add(ObjectType.NONE);

    }};

    public PlayerShotPacket() {
        super(Type.PLAYER_SHOT);
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

        if (!Main.players.containsKey(objects.get(0)) || objects.get(0) != getPlayer().getId() || !ts.contains(objects.get(1))) return;

        Player p = Main.players.get(objects.get(0));

        if (p.getGun() == Gun.ARMS) arms(objects);
        else gun(objects);

    }

    private void arms(List<Object> objects){
        ObjectType t = (ObjectType) objects.get(1);
        Integer oid = (Integer) objects.get(2);

        Boolean dead = false;

        if (t == ObjectType.DESTRUCTIBLE){
            if (!Main.map.objects.containsKey(oid)) return;

            Destructible d = (Destructible) Main.map.objects.get(oid);
            d.setHealth(d.getHealth() - 1);

            dead = d.thatsAll();

        } else if (t == ObjectType.PLAYER){
            if (!Main.players.containsKey(oid)) return;

            Player p = Main.players.get(oid);
            p.setHealth(p.getHealth() - 1);

            dead = p.thatsAll();

        }

        for(Player p : Main.players.values()){

            Utils.packetInstance(PlayerShotPacket.class, p).write(objects.toArray());
            if (dead) Utils.packetInstance(ObjectDespawnPacket.class, p).write(oid, t);

        }

    }

    private void gun(List<Object> objects){
        Float degrees = (Float) objects.get(3);
        Player p = Main.players.get(objects.get(0));

        new Bullet(Main.ider.next(), p.getPosition(), degrees);
        p.setBullets(p.getBullets() - 1);

    }

}
