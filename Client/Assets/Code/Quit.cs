using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Quit : MonoBehaviour
{

    public void reload() {

        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Client>().Disconnect();
        SceneManager.LoadScene("game");

    }

    public void quit() {

        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Client>().Disconnect();
        SceneManager.LoadScene("menu");

    }

}
