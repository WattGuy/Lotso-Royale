using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Items
{

    public Dictionary<Item, GameObject> items = null;

    public Items() {
        items = new Dictionary<Item, GameObject>();

        foreach (Item i in Enum.GetValues(typeof(Item)).Cast<Item>())
        {
            GameObject go = (GameObject) Resources.Load("Items/" + i.ToString());
            if (go == null) continue;

            items.Add(i, go);

        }

    }
   
}
	
