using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public VisualElement m_healthBar;
    public static UIHandler instance;
    // Start is called before the first frame update
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        m_healthBar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        SetHealthValue(1.0f);
    }

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    public void SetHealthValue(float percentage)
    {
        m_healthBar.style.width = Length.Percent(percentage * 100.0f);
    }
}
