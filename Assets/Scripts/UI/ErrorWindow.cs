using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI errorBar;
    [SerializeField] TextMeshProUGUI errorMessage;
    [SerializeField] TextMeshProUGUI errorConfirm;
    [SerializeField] Button confirmButton;

    public void SetText(string bar, string context, string confirm, System.Action confirmAction)
    {
        errorBar.text = bar;
        errorMessage.text = context;
        errorConfirm.text= confirm;
        confirmButton.onClick.AddListener(() => { confirmAction(); });
    }

}
