package me.wattguy.lotso.managers;

import me.wattguy.lotso.Physics;
import me.wattguy.lotso.enums.CircleType;
import me.wattguy.lotso.enums.ObjectType;
import me.wattguy.lotso.utils.Circle;
import me.wattguy.lotso.objects.Destructible;
import me.wattguy.lotso.objects.GameObject;
import me.wattguy.lotso.objects.Player;
import me.wattguy.lotso.packets.CircleInfoPacket;
import me.wattguy.lotso.packets.ObjectSpawnPacket;
import me.wattguy.lotso.utils.Utils;
import me.wattguy.lotso.utils.Vector2;

import javax.swing.*;
import java.util.HashMap;

public class MapManager {

    public Physics p;
    public HashMap<Integer, GameObject> objects = new HashMap<>();
    public Circle circle;

    public void initialize(){

        p = new Physics();
        p.run();

        p.world.getSettings().setStepFrequency(0.001d);

        circle = new Circle(new Vector2(0f, 0f), 3f, CircleType.ZONE, null, false);

    }

    public void onConnect(Player p){

        sendCircle(p);

        for(GameObject go : objects.values()){

            sendGameObject(p, go);

        }

    }

    public void sendGameObject(Player p, GameObject go){

        ObjectType ot = ObjectType.fromObject(go);

        Utils.packetInstance(ObjectSpawnPacket.class, p).write(go.getId(), ot, Utils.getType(go, ot), go.getPosition().x, go.getPosition().y, (ot == ObjectType.DESTRUCTIBLE ? ((Destructible) go).getHealth() : 0));

    }

    public void sendCircle(Player p){

        Utils.packetInstance(CircleInfoPacket.class, p).write(circle.getPosition().x, circle.getPosition().y, circle.getScale());

    }

}
