package me.wattguy.lotso.objects;

import me.wattguy.lotso.Handler;
import me.wattguy.lotso.Main;
import me.wattguy.lotso.enums.CircleType;
import me.wattguy.lotso.enums.Gun;
import me.wattguy.lotso.enums.ItemType;
import me.wattguy.lotso.enums.ObjectType;
import me.wattguy.lotso.packets.ObjectDespawnPacket;
import me.wattguy.lotso.packets.PlayerInfoPacket;
import me.wattguy.lotso.utils.*;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

public class Player extends GameObject {

    public static final int PICKUP_TIME = 1500;

    public static Ider ider = new Ider();
    private Handler h;
    private Long l;
    private ScheduledExecutorService e;
    private Rotation rotation;
    private Gun gun = Gun.ARMS;
    private Integer helmet = 0;
    private Integer health = 100;
    private Integer bullets = 0;
    private Boolean dead = false;
    private Circle c;

    public HashMap<Integer, Integer> staying = new HashMap<>();

    public Player(Integer id, Vector2 position, Rotation rotation){
        super(id, position);

        this.l = System.currentTimeMillis() - 1000;
        this.rotation = rotation;
        this.c = new Circle(position, 1f, CircleType.PLAYER, this, true);

        Player p = this;

        e = Executors.newSingleThreadScheduledExecutor();
        e.scheduleAtFixedRate(() -> {
            if (!Main.players.containsKey(p.getId())) {e.shutdown(); return;}

            if (getDifference() >= Main.TIMEOUT){

                h.ctx.disconnect();
                return;

            }

            HashMap<Integer, Integer> map = new HashMap<>();
            for(GameObject go : Main.map.objects.values()){
                if (!(go instanceof Item && ((Item) go).getCircle().inByDistance(p.getPosition(), .5f))) continue;

                int ms = 100;
                if (p.staying.containsKey(go.getId())) ms += p.staying.get(go.getId());

                map.put(go.getId(), ms);
                p.staying.put(go.getId(), ms);
            }

            List<Integer> remove = new ArrayList<>();
            for(Integer key : p.staying.keySet()){ if (!map.containsKey(key)) remove.add(key); }
            for(Integer key : remove){ p.staying.remove(key); }

        }, 100, 100, TimeUnit.MILLISECONDS);

    }

    public Long getDifference(){
        return System.currentTimeMillis() - l;
    }
    public void setTime(Long l){
        this.l = l;
    }

    @Override
    public void setPosition(Vector2 position) {
        super.setPosition(position);

        c.setPosition(position);
    }

    public Handler getHandler(){
        return h;
    }
    public void setHandler(Handler h){
        this.h = h;
    }

    public Rotation getRotation(){
        return rotation;
    }
    public void setRotation(Rotation rotation){
        this.rotation = rotation;
    }

    public Gun getGun(){
        return gun;
    }
    public void setGun(Gun gun){
        this.gun = gun;
    }

    public Integer getHelmet(){
        return helmet;
    }
    public void setHelmet(Integer helmet){
        this.helmet = helmet;
    }

    public Integer getHealth() {
        return health;
    }
    public void setHealth(Integer health) {

        if (health >= 100)
            this.health = 100;
        else if (health <= 0) {
            this.health = 0;

            if (!dead){

                Utils.writeAll(ObjectDespawnPacket.class, getId(), ObjectType.PLAYER);
                dead = true;

            }

        }else this.health = health;

    }

    public Integer getBullets() {
        return bullets;
    }
    public void setBullets(Integer bullets) {
        this.bullets = bullets;

        if (bullets <= 0){

            gun = Gun.ARMS;

        }

        Utils.writeAll(PlayerInfoPacket.class, getId(), gun, helmet, health, bullets);

    }

    public Boolean thatsAll(){
        return health <= 0;
    }
    
    public void pickupItem(ItemType it) {
    	
    	switch(it){

        	case M92:
        	case AK47:
        		setGun(Gun.fromString(it.toString()));
        	case BULLETS:
        		bullets();
        		break;

        	case BANDAGES:
        		setHealth(health + 20);
        		break;

        	case MEDKIT:
        		setHealth(health + 80);
        		break;

        	case HELMET1:
        	case HELMET2:
        		setHelmet(Integer.parseInt(it.toString().replace("HELMET", "")));
        		break;

    	}
    	
    	Utils.writeAll(PlayerInfoPacket.class, getId(), gun, helmet, health, bullets);
    	
    }

    public Boolean pickupItem(Item i){
        if(!staying.containsKey(i.getId())) return false;

        Integer ms = staying.get(i.getId());
        if (ms < PICKUP_TIME) return false;

        pickupItem(i.getType());

        i.getCircle().dispose();

        return true;
    }

    private void bullets(){

        if(gun == Gun.M92)
            bullets += 15;
        else if (gun == Gun.AK47)
            bullets += 20;
        else
            bullets = 0;

    }

    public Circle getCircle() {
        return c;
    }

}
