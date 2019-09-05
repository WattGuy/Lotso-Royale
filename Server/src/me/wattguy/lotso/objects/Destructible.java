package me.wattguy.lotso.objects;

import me.wattguy.lotso.Main;
import me.wattguy.lotso.enums.DestType;
import me.wattguy.lotso.enums.ObjectType;
import me.wattguy.lotso.framework.SimulationBody;
import me.wattguy.lotso.packets.ObjectSpawnPacket;
import me.wattguy.lotso.utils.Circle;
import me.wattguy.lotso.utils.Utils;
import me.wattguy.lotso.utils.Vector2;
import org.dyn4j.dynamics.BodyFixture;
import org.dyn4j.geometry.Geometry;
import org.dyn4j.geometry.MassType;

import java.awt.*;

public class Destructible extends GameObject {

    private DestType t;
    private Integer health;
    private SimulationBody b;

    public Destructible(Integer id, Vector2 position, DestType t) {
        super(id, position);

        this.t = t;
        this.health = t.getHealth();
        spawn();
    }

    public DestType getType() {
        return t;
    }

    public Integer getHealth(){
        return health;
    }
    public void setHealth(Integer health){
        this.health = health;



        if (thatsAll()) onDestroy();
        else {
            dispose();
            spawn();
        }
    }

    private void spawn(){

        int i = t.getHealth() - health;
        b = new SimulationBody();
        BodyFixture bf = null;
        Color color = new Color(0, 0, 0);
        if(t == DestType.CRATE){

            float f = .95f * Circle.CRATE_RADIUS;
            bf = new BodyFixture(Geometry.createRectangle(f - (i * 0.05f), f - (i * 0.05f)));
            color = new Color(180, 146, 61);

        }else if (t == DestType.STONE){

            bf = new BodyFixture(Geometry.createCircle(1.539672 * Circle.STONE_RADIUS - (i * 0.05f)));
            color = new Color(106, 106, 106);

        }

        bf.setSensor(true);
        bf.setUserData(this);
        b.setColor(color);
        b.addFixture(bf);
        b.setMass(MassType.NORMAL);
        b.translate(getPosition().x, getPosition().y);
        Main.map.p.world.addBody(b);

    }

    private void onDestroy(){

        dispose();

        if (t == DestType.CRATE){

            Integer id = Main.ider.next();
            Item i = new Item(id, getPosition(), Utils.getRandomItem());
            Main.map.objects.put(id, i);

            Utils.writeAll(ObjectSpawnPacket.class, id, ObjectType.ITEM, i.getType().toString(), i.getPosition().x, i.getPosition().y, 0);

        }

        Main.map.objects.remove(getId());
        Main.ider.add(getId());

    }

    public void dispose(){

        if (b != null && b.getFixtureCount() > 0){


            b.removeAllFixtures();
            Main.map.p.world.removeBody(b);

        }

    }

    public Boolean thatsAll(){
        return health <= 0;
    }

}
