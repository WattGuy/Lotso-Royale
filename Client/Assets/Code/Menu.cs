using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    public static string ip = "127.0.0.1";
    public static ushort port = 4296;

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

        ipText.text = ip;
        portText.text = port.ToString();

    }

    public void Connect() {

        ip = ipText.text;
        port = ushort.Parse(portText.text);

        SceneManager.LoadScene("game");

    }

}
