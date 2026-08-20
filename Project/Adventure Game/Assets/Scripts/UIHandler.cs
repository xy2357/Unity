using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public VisualElement m_healthBar;
    public static UIHandler instance;

    public float displayTime = 4.0f;
    private VisualElement m_NonPlayerDialogue;
    private float m_TimeDisplay;
    // Start is called before the first frame update
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        m_healthBar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        SetHealthValue(1.0f);

        m_NonPlayerDialogue = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
        m_NonPlayerDialogue.style.display = DisplayStyle.None;
        m_TimeDisplay = -1.0f;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (m_TimeDisplay > 0)
        {
            m_TimeDisplay -= Time.deltaTime;
            if (m_TimeDisplay < 0)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None;
            }
        }
    }

    // Update is called once per frame
    public void SetHealthValue(float percentage)
    {
        m_healthBar.style.width = Length.Percent(percentage * 100.0f);
    }

    public void DisplayDialogue()
    {
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        m_TimeDisplay = displayTime;
    }
}
