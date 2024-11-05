using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool isMatched = false;
    public GameObject front;
    public GameObject back;
    public List<Material> cardMaterials;
    public Material backMaterial;
    public Animator animator;
    public bool selected = false;
    public GameManager gm;
    
    private int cardType;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
    }

    // En caso de que el player de clic a la carta
    public void OnMouseDown()
    {
        ShowCard();
    }

    // Funcion para asignar el numero a la carta y ponerle el material
    public void SetCard(int type)
    {
        cardType = type;
        front.GetComponent<MeshRenderer>().material = backMaterial;
        back.GetComponent<MeshRenderer>().material = cardMaterials[type];
    }

    // Funcion enviar al GameManager que una carta a sido seleccionada
    public void ShowCard()
    {
        // Comprobacion de si esta carta ya esta matcheada o de si esta ya seleccionada
        if (isMatched || selected) 
            return;
        
        // Pasamos la carta al GameManager conforme a sido seleccionada
        gm.SelectCard(GetComponent<Card>());
    }

    // Funcion para ocultar la carta
    public void HideCard()
    {
        // Ejecutamos la animacion de ocultar la carta
        animator.SetTrigger("Hide");
        // Ponemos la carta como no seleccionada
        selected = false;
    }

    // Funcion para hacer desaparecer la carta cuando se haga match
    public void MatchCard()
    {
        // Ejecutamos la animacion de Match
        animator.SetTrigger("Match");
        // Ponemos la carta como match
        isMatched = true;
        // Destruimos el objeto luego de 1.5 segundos para dar timepo a que haga la animacion
        Destroy(gameObject, 1.5f);
    }

    // Funcion para obtener el numero que tiene asignado la carta
    public int GetType()
    {
        return cardType;
    }
}