package me.wattguy.lotso;

import me.wattguy.lotso.enums.ObjectType;
import me.wattguy.lotso.framework.SimulationFrame;
import me.wattguy.lotso.objects.Bullet;
import me.wattguy.lotso.objects.Destructible;
import me.wattguy.lotso.objects.GameObject;
import me.wattguy.lotso.objects.Player;
import me.wattguy.lotso.packets.ObjectDespawnPacket;
import me.wattguy.lotso.packets.ObjectInfoPacket;
import me.wattguy.lotso.packets.PlayerInfoPacket;
import me.wattguy.lotso.utils.Utils;
import org.dyn4j.dynamics.*;

import com.jogamp.opengl.GLAutoDrawable;

import java.util.AbstractMap;
import java.util.Map;

public class Physics extends SimulationFrame {

    public ContactListener listener;

    public Physics() {
        super("", 32);
    }

    protected void initializeWorld() {
        this.world.setGravity(World.ZERO_GRAVITY);

        listener = new ContactListener();

        this.world.addListener(listener);

    }

    public static class ContactListener extends CollisionAdapter {

        @Override
        public boolean collision(Body body, BodyFixture bodyFixture, Body body1, BodyFixture bodyFixture1) { on(bodyFixture, bodyFixture1); return true; }

        public void on(BodyFixture bf1, BodyFixture bf2){
            Map.Entry<Bullet, GameObject> entry = get(bf1, bf2);

            if (entry == null) return;

            entry.getKey().dispose();

            if (entry.getValue() instanceof Destructible){

                Destructible d = (Destructible) entry.getValue();
                d.setHealth(d.getHealth() - 1);

                boolean dead = d.thatsAll();

                for(Player p : Main.players.values()){

                    if (dead) Utils.packetInstance(ObjectDespawnPacket.class, p).write(d.getId(), ObjectType.DESTRUCTIBLE);
                    else Utils.packetInstance(ObjectInfoPacket.class, p).write(d.getId(), d.getHealth());

                }


            }else if (entry.getValue() instanceof Player){

                Player p = (Player) entry.getValue();
                p.setHealth(p.getHealth() - 6);

                boolean dead = p.thatsAll();

                for(Player o : Main.players.values()){

                    if (dead) Utils.packetInstance(ObjectDespawnPacket.class, p).write(p.getId(), ObjectType.PLAYER);
                    else if (o == p) Utils.packetInstance(PlayerInfoPacket.class, p).write(p.getId(), p.getGun(), p.getHelmet(), p.getHealth(), p.getBullets());

                }

            }

        }

    }

    public static Map.Entry<Bullet, GameObject> get(BodyFixture bf, BodyFixture bf2){

        if (bf.getUserData() != null && bf.getUserData() instanceof Bullet && bf2.getUserData() != null && isObject(bf2.getUserData())) return new AbstractMap.SimpleEntry<>((Bullet) bf.getUserData(), (GameObject) bf2.getUserData());
        else if (bf2.getUserData() != null && bf2.getUserData() instanceof Bullet && bf.getUserData() != null && isObject(bf.getUserData())) return new AbstractMap.SimpleEntry<>((Bullet) bf2.getUserData(), (GameObject) bf.getUserData());

        return null;

    }

    private static Boolean isObject(Object o){

        return o instanceof Player || o instanceof Destructible;

    }

	@Override
	public void dispose(GLAutoDrawable arg0) {
		// TODO Auto-generated method stub
		
	}

}