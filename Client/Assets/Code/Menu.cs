using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    private Text portText;
    private Text ipText;

    void Start()
    {

        foreach (Transform child in GameObject.FindGameObjectWithTag("Canvas").transform) {

            if (child.name == "Port") {

                foreach (Transform child2 in child) {

                    if (child2.name == "Text") portText = child2.GetComponent<Text>();

                }

            }else if (child.name == "IP")
            {

                foreach (Transform child2 in child)
                {

                    if (child2.name == "Text") ipText = child2.GetComponent<Text>();

                }

            }

        }

        ipText.text = Client.ip;
        portText.text = Client.port.ToString();

    }

    public void Connect() {

        Client.ip = ipText.text;
        Client.port = short.Parse(portText.text);

        SceneManager.LoadScene("game");

    }

}
