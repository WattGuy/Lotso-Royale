using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Quit : MonoBehaviour
{

    public void reload() {

        Client.instance.Disconnect(false);
        SceneManager.LoadScene("game");

    }

    public void quit() {

        Client.instance.Disconnect(false);
        SceneManager.LoadScene("menu");

    }

}
