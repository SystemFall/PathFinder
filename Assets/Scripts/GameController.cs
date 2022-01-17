using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject controlController;
    public GameObject screen;

    private Color colorRed;
    private Color colorGreen;
    void Start()
    {
        colorRed = new Color32(255, 81, 0, 255);
        colorGreen = new Color32(120, 255, 0, 255);
        if (SystemInfo.deviceType == DeviceType.Handheld)
            controlController.GetComponent<TouchControl>().enabled = true;
        else
            controlController.GetComponent<NonTouchControl>().enabled = true;
    }
    public void ShowEndScreen(bool mode)
    {
        screen.SetActive(true);
        Text text = screen.transform.Find("Text").GetComponent<Text>();
        if (mode)
        {
            text.color = colorGreen;
            text.text = "Complete";
        }
        else
        {
            text.color = colorRed;
            text.text = "Defeat";
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
