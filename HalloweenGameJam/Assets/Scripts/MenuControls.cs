using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControls : MonoBehaviour
{
    SceneChange sceneChange;
    [SerializeField]GameObject instructionPanel;
    // Start is called before the first frame update
    void Start()
    {
        sceneChange = GetComponent<SceneChange>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            sceneChange.StartGame();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            InstructionPanelToggle();
        }
    }

    public void InstructionPanelToggle()
    {
        instructionPanel.SetActive(!instructionPanel.activeSelf);
    }
}
