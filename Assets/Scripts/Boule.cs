using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Objet représentant une boule contrôlée par le joueur.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Boule : MonoBehaviour
{
    [SerializeField, Tooltip("La cible pour le suvi de la caméra")]
    private Transform cibleCamera;

    [SerializeField, Tooltip("Force de déplacement de la boule.")]
    private float forceDeplacement;

    [SerializeField, Tooltip("Force de d'accélération de la boule.")]
    private float forceAcceleration = 15f;

    // Force appliquée à la boule pour le déplacement à chaque frame.
    private Vector3 forceAppliquee;

    // Référence au Rigidbody de la boule pour appliquer la physique.
    private Rigidbody rigidbody;

    // Référence au Rigidbody de la boule pour appliquer la physique.
    private GameObject prefabCharge;

    /// <summary>
    /// Obtient la vélocité actuelle de la boule.
    /// </summary>
    public Vector3 Velocite => rigidbody.linearVelocity;

    private bool DirectionActif = false;

    private int nombreDeChargeAcceleration = 0;

    private bool chargeEnCours = false;

    public GameObject[] charges;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        ControleurJeu.Instance.Controles.actions.FindAction("Commencer").performed += CommencerJeu;

        PlayerInput controles = ControleurJeu.Instance.Controles;

        if (controles == null)
            return;

        controles.actions.FindAction("Diriger").performed += CommencerDirection;
        controles.actions.FindAction("Diriger").canceled += ArreterDirection;
        controles.actions.FindAction("Accelerer").canceled += Accelerer;
    }

    private void OnDestroy()
    {
        if (ControleurJeu.Instance == null)
            return;
        ControleurJeu.Instance.Controles.actions.FindAction("Commencer").performed -= CommencerJeu;

        PlayerInput controles = ControleurJeu.Instance.Controles;

        if (controles == null) 
            return;

        controles.actions.FindAction("Diriger").performed -= CommencerDirection;
        controles.actions.FindAction("Diriger").canceled -= ArreterDirection;
        controles.actions.FindAction("Accelerer").canceled -= Accelerer;
        
    }

    private void Update()
    {
        if (cibleCamera != null)
        {
            cibleCamera.position = rigidbody.position;
        }
    }

    private void FixedUpdate()
    {
        Diriger();
    }

    private void CommencerDirection(InputAction.CallbackContext contexte)
    {
        forceAppliquee += contexte.ReadValue<float>() * forceDeplacement * Vector3.right;
    }

    private void ArreterDirection(InputAction.CallbackContext contexte)
    {
        forceAppliquee = Vector3.zero;
    }

    private void Diriger()
    {
        if (!DirectionActif)
            return;

        if(!Mathf.Approximately(forceAppliquee.sqrMagnitude, 0.0f))
        {
            rigidbody.AddForce(forceAppliquee, ForceMode.Force);
        }
    }

    /// <summary>
    /// Methode pour commencer le jeu
    /// source : https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Rigidbody-useGravity.html (pour use gravity)
    /// </summary>
    private void CommencerJeu(InputAction.CallbackContext contexte)
    {
        // On ne peut commencer le jeu qu'une seule fois 
        // je me suis inspiré de la méthode commecerJeu dans informationInterface.cs
        ControleurJeu.Instance.Controles.actions.FindAction("Commencer").performed -= CommencerJeu;
        rigidbody.useGravity = true;
        DirectionActif = true;
    }

    /// <summary>
    /// Incrementer le nombre de charge d'acceleration de la boule
    /// </summary>
    public void IncrementerChargeAcceleration()
    {
        if (nombreDeChargeAcceleration <= 2)
        {
            nombreDeChargeAcceleration++;
        }   
    }

    /// <summary>
    /// Acceler la balle de 15 unite pendant une seconde
    /// </summary>
    private void Accelerer(InputAction.CallbackContext contexte)
    {
        if (!DirectionActif)
            return;

        if (nombreDeChargeAcceleration > 0)
        {
            if (!chargeEnCours)
            {
                StartCoroutine(ForceDeDeplacementAccelerSurUneSeconde());
            }    
        }   
    }


 
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ForceDeDeplacementAccelerSurUneSeconde()
    {
        chargeEnCours = true;
        float forceInitiale = forceDeplacement;
        forceDeplacement = forceAcceleration;
        yield return new WaitForSeconds(1.0f);
        forceDeplacement = forceInitiale;
        nombreDeChargeAcceleration--;
        chargeEnCours = false;
    }

    /// <summary>
    /// Methode pour ajouter des charges dans l'interface
    /// on compte le nombre de charge presenrt dans la scene
    /// on compare au nombre de harge cumuler 
    /// on ajoute le nombre manquant
    /// source: https://discussions.unity.com/t/how-do-i-create-a-list-of-all-objects-in-scene-with-a-tag/212189
    /// </summary>
    private void InstancierChargeDansInterface()
    {
        charges = GameObject.FindGameObjectsWithTag("charge");

        int difference = nombreDeChargeAcceleration - charges.Length;

        for (int i = 0; i < difference; i++) {
            GameObject charge = Instantiate(prefabCharge, transform.position, Quaternion.identity);
        }
    }
}
