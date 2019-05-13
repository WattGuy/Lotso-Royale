using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener : MonoBehaviour {

    private PlayerController pc = null;

    private void Start() {

        pc = transform.parent.parent.GetComponent<PlayerController>();

    }

    public void shot() {

        if (pc.d.col != null) {

            pc.playGun();

        }

    }

}
