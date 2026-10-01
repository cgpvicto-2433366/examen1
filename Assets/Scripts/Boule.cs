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

    // Force appliquée à la boule pour le déplacement à chaque frame.
    private Vector3 forceAppliquee;

    // Référence au Rigidbody de la boule pour appliquer la physique.
    private Rigidbody rigidbody;

    /// <summary>
    /// Obtient la vélocité actuelle de la boule.
    /// </summary>
    public Vector3 Velocite => rigidbody.linearVelocity;

    private bool DirectionActif = false;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        ControleurJeu.Instance.Controles.actions.FindAction("Commencer").performed += CommencerJeu;

        PlayerInput controles = ControleurJeu.Instance.Controles;

        if (controles == null)
            return;

        controles.actions.FindAction("Diriger").performed += CommencerDirection;
        controles.actions.FindAction("Diriger").canceled += ArreterDirection;
    }

    private void OnDestroy()
    {
        if (ControleurJeu.Instance == null)
            return;

        PlayerInput controles = ControleurJeu.Instance.Controles;

        if (controles == null) 
            return;

        controles.actions.FindAction("Diriger").performed -= CommencerDirection;
        controles.actions.FindAction("Diriger").canceled -= ArreterDirection;
        ControleurJeu.Instance.Controles.actions.FindAction("Commencer").performed -= CommencerJeu;
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
}
