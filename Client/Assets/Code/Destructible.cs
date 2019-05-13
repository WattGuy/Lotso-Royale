using UnityEngine;

public class Destructible : MonoBehaviour {

    public ushort id;
    public int max = 11;
    public float step = 0.05f;
    public int shots = 0;

    private Vector3 scale;
    private bool b = false;

    public void setShots(int shots) {

        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - (0.05f * (shots - this.shots)), gameObject.transform.localScale.y - (0.05f * (shots - this.shots)), 0f);

        this.shots = shots;

    }

}
