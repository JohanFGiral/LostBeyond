using UnityEngine;

public class TorchController : MonoBehaviour
{
    public Sprite spriteOn;
    public Sprite spriteOff;
    public int torchNumber; // El número que esta antorcha representa

    private SpriteRenderer spriteRenderer;
    private bool isLit = false;
    private PasswordManager passwordManager;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("TorchController necesita un componente SpriteRenderer en " + gameObject.name);
            enabled = false;
            return;
        }
        // Usar el método actualizado para encontrar objetos
        passwordManager = Object.FindFirstObjectByType<PasswordManager>();
        if (passwordManager == null)
        {
            Debug.LogError("No se encontró un PasswordManager en la escena.");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        SetState(false); // Asegurarse de que la antorcha comience apagada
    }

    // Este método ahora es público y será llamado por el script del botón numérico
    public void ActivateTorch()
    {
        if (enabled && !isLit) // Solo reaccionar si la antorcha no está ya encendida
        {
            SetState(true);
            passwordManager.TorchActivated(torchNumber, this); // Notificar al PasswordManager
        }
        // Si quieres que al tocar de nuevo una antorcha encendida se apague (y se quite de la secuencia),
        // necesitarías añadir lógica aquí y en PasswordManager para manejar la desactivación.
        // Por ahora, una vez encendida, solo se apaga con un reset.
    }

    // Método para cambiar el estado visual y lógico de la antorcha
    public void SetState(bool lit)
    {
        isLit = lit;
        if (spriteRenderer != null) // Chequeo adicional
        {
            spriteRenderer.sprite = lit ? spriteOn : spriteOff;
        }
    }

    public bool IsLit()
    {
        return isLit;
    }
}