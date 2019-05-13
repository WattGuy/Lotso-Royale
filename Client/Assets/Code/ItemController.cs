using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ItemController : MonoBehaviour {

    public ushort id;
    public GameObject player = null;
    public float speed = 0.02f;
    public Item item = Item.M92;
    private SpriteRenderer progress = null;
    private bool all = false;

    void Start() {

        progress = GetComponent<SpriteRenderer>();

    }

    void FixedUpdate () {
        float f = progress.material.GetFloat("_Fillpercentage");

        if (player != null && player.GetComponent<PlayerController>() != null && player.GetComponent<PlayerController>().controllable == true)
        {
            if (all) progress.material.SetFloat("_Fillpercentage", 0f);
            f += speed;

            if (f >= 1)
            {

                progress.material.SetFloat("_Fillpercentage", 1f);
                PlayerController pc = player.GetComponent<PlayerController>();

                pc.client.Send(new PlayerPickupMessage(id), 0, ENet.PacketFlags.Reliable);

                all = true;

            }
            else
            {

                progress.material.SetFloat("_Fillpercentage", f);

            }

        }
        else if (f > 0){

            progress.material.SetFloat("_Fillpercentage", 0f);
            all = false;

        }
		
	}

    void OnTriggerStay2D(Collider2D col) {
        PlayerController pc = col.gameObject.GetComponent<PlayerController>();

        if (pc != null) {
            Helmet? h = pc.IsItHelmet(item);

            if (pc.gun == Gun.ARMS && item == Item.BULLETS) {

                return;

            } else if (pc.health / 100f >= 1f && (item == Item.BANDAGES || item == Item.MEDKIT)) {

                return;

            } else if (h != null && (int) h <= (int) pc.helmet) {

                return;

            }

            player = col.gameObject;

        }

    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == player)
        {

            player = null;

        }

    }

}
