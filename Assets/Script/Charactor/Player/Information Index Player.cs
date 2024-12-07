using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationIndexPlayer : MonoBehaviour
{
    SingletonIndexPlayer DataPlayer;

    [SerializeField] private Image Hp_bar;
    [SerializeField] private Image Stamina_bar;

    // Start is called before the first frame update
    void Start()
    {
        DataPlayer = SingletonIndexPlayer.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        SetUIPlayer();
    }

    public void SetUIPlayer()
    {
        Hp_bar.fillAmount = DataPlayer.Health / DataPlayer.Max_Health;
        Stamina_bar.fillAmount = DataPlayer.Stamina / DataPlayer.Max_Stamina;
    }
}
