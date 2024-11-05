using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public enum State
{
    SetupTable,
    SelectCard1,
    SelectCard2,
    MatchCards,
    End
}

public class GameManager : MonoBehaviour
{
    public List<GameObject> cards;
    public int totalMatches = 8;
    public GameObject uiPanel;
    public GameObject gameOverPanel;
    public TMP_Text uiTriesText;
    public TMP_Text uiScoreText;
    public TMP_Text uiTotalTime;
    public TMP_Text uiBestTime;
    public TMP_Text timeText;
    public TMP_Text bestTimeText;
    public GameObject congratsText;
    public AudioClip matchAudio;
    public AudioClip notMatchAudio;
    public AudioClip flipAudio;
    public AudioClip winAudio;
    public AudioClip looseAudio;
    
    private Card firstCard;
    private Card secondCard;
    private int matches = 0;
    private int tries = 0;
    private State state;
    private bool first = false;
    private bool second = false;
    private AudioController audioController;
    private bool playEndAudio = true;
    
    private float startTime;
    private float elapsedTime; 
    
    void Start()
    {
        uiScoreText.SetText("Matches: " + matches + "/" + totalMatches);
        uiTriesText.SetText("Tries: " + tries);
        
        // Ponemos el best score en la UI para que lo vea el usuario
        float bestTime = PlayerPrefs.GetFloat("BestTime", 0);
        int minutes = Mathf.FloorToInt(bestTime / 60);
        int seconds = Mathf.FloorToInt(bestTime % 60);
        
        uiBestTime.SetText("Best time: " + minutes.ToString("00") + ":" + seconds.ToString("00"));
        
        // Ponemos el estado del GameManager para que prepare la paritda
        state = State.SetupTable;
        // Guardamos el timepo en el que ha empezado la partida
        startTime = Time.time;
        audioController = FindObjectOfType<AudioController>();
    }

    void Update()
    {
        // En caso de que el estado del GameManager sea acabar la partida
        if (state != State.End)
        {
            // Calculamos el tiempo total que ha tardado en terminar la partida y lo ponemos en la UI
            elapsedTime = Time.time - startTime;
            
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            
            uiTotalTime.SetText("Total time: " + minutes.ToString("00") + ":" + seconds.ToString("00"));
        }
        
        switch (state)
        {
            case State.SetupTable:
            {
                // Ejecutamos la funcion que prepara toda la mesa de juego
                SetupTable();
                break;
            }
            case State.MatchCards:
            {
                // En caso de que el jugador haya seleccionado dos cartas, comprobamos si son del mismo tipo
                if (firstCard.GetType() == secondCard.GetType())
                {
                    // En caso que si sean del mismo tipo ejecutamos la funcion de Match
                    Invoke("Match", 1.5f);
                    // Volvemos a poner el estado en seleccionar la primera carta (La comprobacion de si ya ha hecho match a todos va dentro de la funcion Match)
                    state = State.SelectCard1;
                }
                else
                {
                    // En caso de no ser match ejecutamos la funcion de NotMatch
                    Invoke("NotMatch", 1.5f);
                    // Volvemos a poner el estado en seleccionar la primera carta
                    state = State.SelectCard1;
                }
                break;
            }
            case State.End:
            {
                // Guradmos el timepo que ha hecho el player
                SaveTime();
                // Activamos la pantalla de final de partida y damos 1 segundo para que todas animaciones se ejecuten y no salte de golpe la screen
                Invoke("EnableGameOver", 1.0f);
                break;
            }
        }
    }
    
    public void SetupTable()
    {
        // Generamos una array de numeros aleatorios en la que siempre coincidan dos
        List<int> numeros = GenerarNumerosAleatorios();
        
        // Asignamos un numero a cada carta
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i].GetComponent<Card>();
            card.SetCard(numeros[i]);
        }
        
        // Ponemos en estado en seleccionar la primera carta para poder empezar la partida
        state = State.SelectCard1;
    }
    
    public void SelectCard(Card card)
    {
        // En caso de que la primera y segunda carta ya esten seleccionadas o que el estado del GameManager sea MatchCards paramos este proceso para evitar
        // que seleccionen cartas mientras se comprueba si las cartas seleccionadas son match
        if (first && second || state == State.MatchCards)
            return;
        
        // En caso de que el estado sea seleccionar la primera carta
        if (state == State.SelectCard1)
        {
            // Guardamos el objecto de la carta en nuestra variable
            firstCard = card;
            // Ejecutamos la animacion de mostrar la carta
            firstCard.animator.SetTrigger("Show");
            // Decimos que esta carta ya esta seleccionada
            firstCard.selected = true;
            first = true;
            // Ejecutamos el sonido de girar la carta
            audioController.PlaySound(flipAudio);
            // Ponemos el estado en seleccionar la segunda carta
            state = State.SelectCard2;
        } 
        else if (state == State.SelectCard2)
        {
            // Guardamos el objecto de la carta en nuestra variable
            secondCard = card;
            // Ejecutamos la animacion de mostrar la carta
            secondCard.animator.SetTrigger("Show");
            // Decimos que esta carta ya esta seleccionada
            secondCard.selected = true;
            second = true;
            // Ejecutamos el sonido de girar la carta
            audioController.PlaySound(flipAudio);
            // Ponemos el estado en comprobar si las cartas seleccionadas son match
            state = State.MatchCards;
        }
    }

    private void Match()
    {
        // Sumamos la los intentos de la partida y los mostramos en la UI
        tries++;
        uiTriesText.SetText("Tries: " + tries);
        
        // Ejecutamos la funcion Match de las casrtas para que desaparezcan
        firstCard.MatchCard();
        secondCard.MatchCard();
        first = false;
        second = false;
        
        // Ejecutamos el audios de hacer match
        audioController.PlaySound(matchAudio);
        
        // Sumamos la cuenta de matchs y la ponemos en la UI
        matches += 1;
        uiScoreText.SetText("Matches: " + matches + "/" + totalMatches);
        
        // En caso de que el player haya hecho match a todas las cartas ponemos el estado en partida acabada
        if (matches >= totalMatches)
        {
            state = State.End;
        }
        // Sino continuamos y ponemos el estado en seleccionar la primera carta
        else
        {
            state = State.SelectCard1;
        }
    }
    
    private void NotMatch()
    {
        // Sumamos los intentos y los ponemos en la UI
        tries++;
        uiTriesText.SetText("Tries: " + tries);
        
        // Ocultamos otra vez las cartas
        firstCard.HideCard();
        secondCard.HideCard();
        // Ejecutamos el audio de no hacer match
        audioController.PlaySound(notMatchAudio);
        // Ejecutamos esta funcion un poco mas tarde para dar tiempo a que se haga la animacion de ocultar las cartas
        Invoke("HideCards", 0.8f);
        
        // Ponemos las cartas
        state = State.SelectCard1;
    }

    private void HideCards()
    {
        // Pasamos a false las variables de si estan seleccionadas las cartas para que el player pueda volver a seleccionar las cartas
        first = false;
        second = false;
    }

    // Funcion que devuelve una lista de numeros aleatorios en la que siempre coinciden dos
    private List<int> GenerarNumerosAleatorios()
    {
        List<int> numeros = new List<int>();
        
        for (int i = 0; i <= 7; i++)
        {
            numeros.Add(i);
            numeros.Add(i);
        }
        
        int n = numeros.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = numeros[i];
            numeros[i] = numeros[j];
            numeros[j] = temp;
        }

        return numeros;
    }

    // Funcion para guardar el best score
    private void SaveTime()
    {
        float time = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
                
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        if (elapsedTime < time)
        {
            PlayerPrefs.SetFloat("BestTime", elapsedTime);
        }

        timeText.SetText("Time: " + minutes.ToString("00") + ":" + seconds.ToString("00"));
        
        float bestTime = PlayerPrefs.GetFloat("BestTime", 0);
        
        int bestMinutes = Mathf.FloorToInt(bestTime / 60);
        int bestSeconds = Mathf.FloorToInt(bestTime % 60);
        bestTimeText.SetText("Best time: " + bestMinutes.ToString("00") + ":" + bestSeconds.ToString("00"));
    }

    // Funcion para activar la pantalla de final de la partida
    private void EnableGameOver()
    {
        // Desactivamos la UI de la partida y activamos la pantalla de final de la partida
        uiPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        
        // Obtenemos el best score guardado en la memoria
        float time = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        
        // En caso de que el tiempo que ha hecho el player sea mejor que el guardado
        if (elapsedTime < time)
        {
            // Activamos el mensaje de enhorabuena
            congratsText.SetActive(true);
            
            // Hacemos esto para hacer que el audio solo se ejecute una vez
            if (!playEndAudio)
                return;
            
            // Ejecutamos el audio de victoria
            audioController.PlaySound(winAudio);
            playEndAudio = false;
        }
        // En caso de que el tiempo guardado no haya sido superado por el player
        else
        {
            // Desactivamos el mensaje de enhorabuena (En teoria deberia de estar ya desactivado por defecto)
            congratsText.SetActive(false);
            
            // Hacemos esto para hacer que el audio solo se ejecute una vez
            if (!playEndAudio)
                return;
            
            // Ejecutamos el audio de derrota
            audioController.PlaySound(looseAudio);
            playEndAudio = false;
        }
    }

    // Funcion para el boton de volver al menu
    public void ExitMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    // Funcion para el boton de volver a empezar la partida
    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }
}
