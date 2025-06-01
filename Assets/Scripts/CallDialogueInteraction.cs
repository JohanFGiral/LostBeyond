// CallDialogueInteraction.cs
using UnityEngine;
using System.Collections; // Necesario para Coroutines

public class CallDialogueInteraction : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Arrastra aquí el GameObject que tiene el script 'Dialogue.cs' (ej. Luna_idle_animation_0)")]
    public Dialogue dialogueScriptInstance;

    [Tooltip("El GameObject que se desactivará (ej. Luna_idle_animation_0 si es la 'llave')")]
    public GameObject keyToDeactivate;

    [Header("Trigger Settings")]
    [Tooltip("Etiqueta del objeto jugador.")]
    public string playerTag = "Player";
    [Tooltip("Si es verdadero, este trigger solo iniciará el diálogo la primera vez.")]
    public bool triggerDialogueOnceOnly = true;

    [Header("Deactivation Settings")]
    [Tooltip("Tiempo en segundos a esperar DESPUÉS de iniciar el diálogo antes de desactivar la llave. Ajusta esto según la duración estimada de tu diálogo.")]
    public float delayBeforeDeactivatingKey = 5.0f; // ¡ESTE VALOR ES CRUCIAL!

    private bool dialogueHasBeenTriggeredByThis = false;

    void Awake()
    {
        if (dialogueScriptInstance == null) {
            Debug.LogError("ERROR en '" + gameObject.name + "': 'Dialogue Script Instance' no asignado.");
            enabled = false; return;
        }
        if (keyToDeactivate == null) {
            Debug.LogError("ERROR en '" + gameObject.name + "': 'Key To Deactivate' no asignado.");
            enabled = false; return;
        }
        Collider2D col = GetComponent<Collider2D>();
        if (col == null || !col.isTrigger) {
            Debug.LogError("ERROR en '" + gameObject.name + "': Necesita un Collider2D marcado como 'Is Trigger'.");
            enabled = false; return;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (triggerDialogueOnceOnly && dialogueHasBeenTriggeredByThis)
            {
                return;
            }

            if (dialogueScriptInstance != null && keyToDeactivate != null && keyToDeactivate.activeInHierarchy)
            {
                Debug.Log("TRIGGER AUTOMÁTICO '" + gameObject.name + "' llamando a ActivarInteraccionDesdeBoton() en '" + dialogueScriptInstance.gameObject.name + "'.");
                dialogueScriptInstance.ActivarInteraccionDesdeBoton(); // Inicia el diálogo

                // Iniciar la corutina para desactivar la llave después de un retraso
                StartCoroutine(DeactivateKeyAfterDelay());

                if (triggerDialogueOnceOnly)
                {
                    dialogueHasBeenTriggeredByThis = true;
                    // Desactivar el collider de ESTE trigger para que no se vuelva a activar
                    // mientras esperamos que la llave se desactive.
                    GetComponent<Collider2D>().enabled = false;
                }
            }
        }
    }

    private IEnumerator DeactivateKeyAfterDelay()
    {
        Debug.Log("'" + gameObject.name + "': Esperando " + delayBeforeDeactivatingKey + " segundos para desactivar '" + keyToDeactivate.name + "'.");
        
        // Esperar el tiempo configurado
        yield return new WaitForSeconds(delayBeforeDeactivatingKey);

        // Opcional: Una comprobación más sofisticada antes de desactivar.
        // Podríamos intentar ver si el panel de diálogo del 'dialogueScriptInstance' sigue activo.
        // Esto requiere que 'dialogueScriptInstance' exponga su panel o un estado 'IsDialogueActive'.
        // Ejemplo (requeriría cambios en Dialogue.cs para exponer 'IsDialoguePanelActive()'):
        // while (dialogueScriptInstance.IsDialoguePanelActive()) {
        //    yield return new WaitForSeconds(0.5f); // Revisar cada medio segundo
        // }

        if (keyToDeactivate != null && keyToDeactivate.activeInHierarchy)
        {
            Debug.Log("'" + gameObject.name + "': Retraso completado. Desactivando '" + keyToDeactivate.name + "'.");
            keyToDeactivate.SetActive(false);
        }
        else if (keyToDeactivate != null)
        {
            Debug.Log("'" + gameObject.name + "': Retraso completado, pero '" + keyToDeactivate.name + "' ya estaba inactiva.");
        }

        // Opcional: Si este trigger "Square" ya cumplió toda su función, puedes desactivarlo también.
        // if (triggerDialogueOnceOnly) {
        //    gameObject.SetActive(false);
        // }
    }
}