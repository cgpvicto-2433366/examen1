using UnityEngine;

public class DecteurAcceleration : MonoBehaviour
{
    [SerializeField, Tooltip("Référence à la boule.")]
    private Boule boule;

    /// <summary>
    /// Gestion de la colision entre la boule 
    /// et un objet d'acceleration
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boule"))
        {
            boule.IncrementerChargeAcceleration();
            Destroy(gameObject);
        }
    }
}
