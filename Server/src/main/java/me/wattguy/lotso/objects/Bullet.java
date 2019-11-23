package me.wattguy.lotso.objects;

import me.wattguy.lotso.Main;
import me.wattguy.lotso.enums.CircleType;
import me.wattguy.lotso.framework.SimulationBody;
import me.wattguy.lotso.utils.Utils;
import me.wattguy.lotso.utils.Vector2;
import org.dyn4j.dynamics.BodyFixture;
import org.dyn4j.geometry.Geometry;
import org.dyn4j.geometry.MassType;

import java.awt.*;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

public class Bullet extends GameObject {

    private float rotation;
    private ScheduledExecutorService e;

    private SimulationBody b;

    public Bullet(Integer id, Vector2 position, float rotation) {
        super(id, position);

        this.rotation = rotation;
        spawn();
    }

    private void spawn(){

        b = new SimulationBody(CircleType.BULLET);
        BodyFixture bf = new BodyFixture(Geometry.createRectangle(0.1295, 0.1295));

        bf.setSensor(true);
        bf.setUserData(this);
        b.addFixture(bf);
        b.setMass(MassType.NORMAL);
        org.dyn4j.geometry.Vector2 g = Utils.degreesToVector2(rotation);
        b.translate(getPosition().x + g.x, getPosition().y + g.y);
        b.applyForce(Utils.multiply(g, Main.BULLET_SPEED));
        Main.map.p.world.addBody(b);

        e = Executors.newSingleThreadScheduledExecutor();
        e.schedule(() -> Main.map.p.addInQueue(this::dispose), 1000, TimeUnit.MILLISECONDS);

    }

    public void dispose(){

        if (b != null && b.getFixtureCount() > 0) {
            b.removeAllFixtures();
            Main.map.p.world.removeBody(b);
        }

        if(e != null && !e.isShutdown()) e.shutdown();

    }

    public float getRotation() {
        return rotation;
    }

}
