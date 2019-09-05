using System;
using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour {

    public ushort id;
    public PlayerController by;
    public float speed = 2.4f;
    private Rigidbody2D rb;
    private long when;
    private Vector3 start;
    private bool b;

    void Start() {

        when = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        start = gameObject.transform.position;

        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(WaitAndDestroy());

    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(1);
        if (gameObject != null) Destroy(gameObject);
    }

    void FixedUpdate() {

        rb.AddForce(transform.up * speed, ForceMode2D.Impulse);

    }

    void OnTriggerEnter2D(Collider2D col) {

        if (col.GetComponent<BulletController>() == null && col.GetComponent<Detector>() == null && col.GetComponent<ItemController>() == null && col.name != "Circle")
        {

            Vector3 v = transform.position;

            if (gameObject != null) Destroy(gameObject);

            if (col.gameObject.GetComponent<PlayerController>() != null)
            {

                GameObject.Instantiate(Client.instance.blood, v, Quaternion.identity, col.transform);

            }

        }

    }

}
