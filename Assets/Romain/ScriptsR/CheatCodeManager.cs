using UnityEngine;
using UnityEngine.InputSystem;

public class CheatCodeManager : MonoBehaviour
{
    [Header("Références")]
    public PlayerControllerNew playerController; // ton script player
    public CameraFollow cameraFollow;            // ton script de caméra

    [Header("Valeurs du cheat")]
    public float cheatSpeedValue = 10f;

    private float originalPlayerSpeed;
    private float originalCameraSmooth;
    private bool cheatEnabled = false;

    private PlayerInput playerInput;

    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput != null)
            playerInput.actions["Cheatcode"].performed += ToggleCheat;
        else
            Debug.LogWarning("Aucun PlayerInput trouvé, le cheatcode ne pourra pas être activé.");

        if (playerController != null)
            originalPlayerSpeed = playerController.moveSpeed;

        if (cameraFollow != null)
            originalCameraSmooth = cameraFollow.smoothSpeed;
    }

    private void ToggleCheat(InputAction.CallbackContext context)
    {
        cheatEnabled = !cheatEnabled;

        if (playerController != null)
            playerController.moveSpeed = cheatEnabled ? cheatSpeedValue : originalPlayerSpeed;

        if (cameraFollow != null)
            cameraFollow.smoothSpeed = cheatEnabled ? cheatSpeedValue : originalCameraSmooth;

        Debug.Log($"Cheatcode {(cheatEnabled ? "activé" : "désactivé")} !");
    }

    void OnDestroy()
    {
        if (playerInput != null)
            playerInput.actions["Cheatcode"].performed -= ToggleCheat;
    }
}
