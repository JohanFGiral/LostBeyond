using UnityEngine;

public class NumericButtonActivator : MonoBehaviour
{
    // Arrastra aquí desde el Inspector la antorcha visual (GameObject con TorchController)
    // que este botón debe activar.
    public TorchController targetTorch;

    void Start()
    {
        if (targetTorch == null)
        {
            Debug.LogError("NumericButtonActivator en '" + gameObject.name + "' no tiene un 'Target Torch' asignado.");
            enabled = false; // Deshabilitar si no hay antorcha asignada
        }
    }

    // Este método será llamado por el EventTrigger del botón numérico
    public void OnNumericButtonPressed()
    {
        if (enabled && targetTorch != null)
        {
            targetTorch.ActivateTorch();
        }
    }
}