using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller; // Character controller (� ajouter au perso)
    public Transform cam; // Transform de la cam

    public float speed = 6f; // vitesse de mouvement
    public float sprintSpeed = 14f;
    public float jumpHeight = 10f;
    public float turnSmoothTime = 0.1f; // dampening de la rotation

    float turnSmoothVelocity; // Permet une rotation plus smooth en conjonction avec 
                              // Mathf.SmoothDampAngle(). Garde un oeil sur la velocite 
                              // du changement d'angle pour assurer une transition smooth.

    public float gravity = -20f; // parametre de la gravite
    Vector3 velocity; // velocite du perso en 3D

    public Transform groundCheck; // Transform par rapport au sol
    public float groundDistance = 0.4f; // Distance max du sol
    public LayerMask groundMask; // Layer de detection pour le sol
    bool isGrounded; // Bool pour voir si on touche le sol

    void Update()
    {
        // Detection du sol avec un spherecast. On regarde la position, la distance avec le sol et le layer du sol
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Si on touche le sol et si le personnage descend (tombe)
        if (isGrounded && velocity.y < 0)
        {
            // petite velocite de descente pour eviter de flotter
            velocity.y = -8f;
        }

        // Input pour le mouvement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // passation des variables de mouvement au vecteur3 du perso
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Si on bouge
        if (direction.magnitude >= 0.1f)
        {
            // Calcul de l'angle de rotation en radiants par rapport a l'axe des Z et le point de direction (x et y)
            // Donc calcule la rotation en cercle et fait des quadrant pour la direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            // On smooth la rotation pour eviter les mouvements brusques
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // On applique la rotation au perso
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // On bouge le perso dans la direction de la camera pour que son dos fasse toujours face a la camera
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Regarde si on sprint ou pas
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;

            // On bouge le perso
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }

        // Regarde si on saute
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
        }

        // On applique la gravite au mouvement y
        velocity.y += gravity * Time.deltaTime;

            // Debug log for velocity
            Debug.Log("Velocity Y: " + velocity.y);

        // On applique le mouvement au perso
        controller.Move(velocity * Time.deltaTime);
    }
}