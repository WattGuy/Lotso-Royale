package me.wattguy.lotso.utils;

import me.wattguy.lotso.Main;
import me.wattguy.lotso.enums.CircleType;
import me.wattguy.lotso.framework.SimulationBody;
import org.dyn4j.dynamics.Body;
import org.dyn4j.dynamics.BodyFixture;
import org.dyn4j.dynamics.contact.ContactPoint;
import org.dyn4j.geometry.Geometry;
import org.dyn4j.geometry.Transform;

public class Circle {

    public static final Float PLAYER_RADIUS = .5814f; // .8f
    public static final Float ZONE_RADIUS = 5.12f;
    public static final Float ITEM_RADIUS = 1.13f;
    public static final Float STONE_RADIUS = .61f; // .8f
    public static final Float CRATE_RADIUS = 2.1f; // 2.59f

    private Vector2 position;
    private Float scale;
    private CircleType t;

    private Body body = null;
    private BodyFixture bf = null;
    private Object o = null;
    private Boolean active = true;

    public Circle(Vector2 position, Float scale, CircleType t, Object o, Boolean active){

        this.position = position;
        this.scale = scale;
        this.t = t;
        this.o = o;
        this.active = active;

        setCircle();

    }

    private void setCircle(){
        dispose();

        Main.map.p.addInQueue(() -> {
            SimulationBody circle = new SimulationBody(t);

            bf = new BodyFixture(Geometry.createCircle(typeToRadius(t) * scale));
            bf.setSensor(true);

            if (o != null) bf.setUserData(o);
            circle.addFixture(bf);
            circle.setActive(active);
            circle.translate(position.x, position.y);
            Main.map.p.world.addBody(circle);

            body = circle;
        });

    }

    public Vector2 getPosition() {
        return position;
    }
    public void setPosition(Vector2 position) {
        this.position = position;

        Transform t = new Transform();
        t.setTranslation(position.x, position.y);
        body.setTransform(t);
    }

    public Float getScale() {
        return scale;
    }
    public void setScale(Float scale) {

        this.scale = scale;
        setCircle();

    }

    public Boolean inByContact(Body b){
        if(body == null) return false;

        return body.isInContact(b);

    }

    public Boolean inByDistance(Vector2 v){
        return inByDistance(v, 0f);
    }

    public Boolean inByDistance(Vector2 v, Float plus){
        return Utils.distance(v, position) <= scale * typeToRadius(t) + plus;
    }

    public static Float typeToRadius(CircleType t){

        switch(t){

            case ZONE:
                return ZONE_RADIUS;

            case ITEM:
                return ITEM_RADIUS;

            case PLAYER:
                return PLAYER_RADIUS;

        }

        return 1f;

    }

    public void dispose(){
        if (body != null) {

            body.removeAllFixtures();
            Main.map.p.world.removeBody(body);

        }
    }

    public Body getBody() {
        return body;
    }

    public BodyFixture getBodyFixture() {
        return bf;
    }

}
