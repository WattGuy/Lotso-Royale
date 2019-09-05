package me.wattguy.lotso.objects;

import me.wattguy.lotso.utils.Vector2;

public abstract class GameObject {

    private Integer id;
    private Vector2 position;

    public GameObject(Integer id, Vector2 position){

        this.id = id;
        this.position = position;

    }

    public Integer getId(){
        return id;
    }

    public Vector2 getPosition() {
        return position;
    }
    public void setPosition(Vector2 position) {
        this.position = position;
    }

}
