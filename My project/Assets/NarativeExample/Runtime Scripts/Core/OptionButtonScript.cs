using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionButtonScript : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;

    [SerializeField]
    private Button button;

    private int ID;
    private Action<int> onOptionSelected;

    private void OnOptionSelected()
    {
        onOptionSelected?.Invoke(ID);
    }
    private void OnEnable()
    {
        button.onClick.AddListener(OnOptionSelected);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnOptionSelected);
    }
    public void Initalise(string textContent, int ID, Action<int> optionSelected)
    {
        textMeshProUGUI.text = textContent;
        this.ID = ID;
        this.onOptionSelected = optionSelected;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    


}
