using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour {

    public Collider2D col = null;

    void OnTriggerEnter2D(Collider2D col) {

        this.col = col;

    }

    void OnTriggerStay2D(Collider2D col) {

        this.col = col;

    }

    void OnTriggerExit2D(Collider2D col)
    {

        this.col = null;

    }

}
