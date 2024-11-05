using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public TMP_Text bestTimeText;
    
    // Funcion para el boton de empezar partida
    public void onStartButton()
    {
        SceneManager.LoadScene("Game");
    }
    
    // Funcion para resetear el score
    public void onResetButton()
    {
        PlayerPrefs.SetFloat("BestTime", 360.0f);
        PlayerPrefs.Save();
    }
    
    // El empezar el juego ponemos el best score que hay guardado en el menu
    void Start()
    {
        float bestTime = PlayerPrefs.GetFloat("BestTime", 0);
        
        int minutes = Mathf.FloorToInt(bestTime / 60);
        int seconds = Mathf.FloorToInt(bestTime % 60);
        bestTimeText.SetText("Best time: " + minutes.ToString("00") + ":" + seconds.ToString("00"));
    }
}
