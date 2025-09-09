using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HPDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    public void OnHPChange(float newValue)
    {
        text.text = newValue.ToString();
    }
}
