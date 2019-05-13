using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletController : MonoBehaviour {

    public ushort id;
    public PlayerController by;
    public float speed = 2.4f;
    private Rigidbody2D rb;

    void Start() {

        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(WaitAndDestroy());

    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(2);
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

            Client.instance.bullets.Remove(id);

            if (col.gameObject.GetComponent<PlayerController>() != null)
            {

                GameObject.Instantiate(by.blood, v, Quaternion.identity, col.transform);

            }

            if (!by.controllable) return;

            if (col.gameObject.GetComponent<Destructible>() != null) {

                by.client.Send(new BulletDestructibleHitMessage(id, col.gameObject.GetComponent<Destructible>().id), 0, ENet.PacketFlags.Reliable);

            } else if (col.gameObject.GetComponent<PlayerController>() != null) {

                by.client.Send(new BulletPlayerHitMessage(id, col.gameObject.GetComponent<PlayerController>().id, 15), 0, ENet.PacketFlags.Reliable);
                
            }


        }

    }

}
