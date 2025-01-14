using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour

{
    
    private Slider slider;
    [SerializeField]
    public TextMeshProUGUI healthCounter;

    public GameObject PlayerState;

    private float currentHealth, maxHealth;


    // Start is called before the first frame update
    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = PlayerState.GetComponent<PlayerState>().currentHealth;
        maxHealth = PlayerState.GetComponent<PlayerState>().maxHealth;

        float fillValue = currentHealth / maxHealth ;
        slider.value = fillValue;

        healthCounter.text = currentHealth + "/" + maxHealth;



    }
}
