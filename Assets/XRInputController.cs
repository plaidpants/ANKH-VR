using UnityEngine;
using UnityEngine.InputSystem;

public class XRInputController : MonoBehaviour
{
    private XRInputActions inputActions; // Assuming XRInputActions is the name of the generated class

    private void Awake()
    {
        inputActions = new XRInputActions();

        // Assuming "Select" is the name of your action for activating something
        inputActions.XRController.Select.performed += context => OnSelectPerformed();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnSelectPerformed()
    {
        GetComponent<World>().changeTiles = true;

        // Your select action logic here
        Debug.Log("Select action performed");
    }
}
