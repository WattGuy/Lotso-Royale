using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cartridge : MonoBehaviour
{

    public void destroy() {

        Destroy(this.gameObject.transform.parent.gameObject);

    }

}
