using UnityEngine;

public class Destructible : MonoBehaviour {

    public int id;
    public int health = 11;
    private float step = 0.05f;

    public void SetHealth(int health) {

        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - (0.05f * (this.health - health)), gameObject.transform.localScale.y - (0.05f * (this.health - health)), 0f);

        this.health = health;

    }

}
