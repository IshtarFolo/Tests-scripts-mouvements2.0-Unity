using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // Transform de la cible (le perso)
    public float distance = 5.0f; // Distance de la camera
    public float xSpeed = 120.0f; // Vitesse de rotation en X
    public float ySpeed = 120.0f; // Vitesse de rotation en Y

    public float yMinLimit = -20f; // Limitation de la rotation en Y (MIN)
    public float yMaxLimit = 80f; // Limitation de la rotation en Y (MAX)

    public float distanceMin = .5f; // Distance minimale de la camera
    public float distanceMax = 15f; // Distance maximale de la camera

    public LayerMask collisionLayers = -1; // Layer de collision
    public float zoomDampening = 5.0f; // Dampening du zoom
    public float minDistance = 0.6f; // Distance minimale de la camera pour zoom et dampening
    public float maxDistance = 20f; // Distance maximale de la camera pour zoom et dampening
    public float targetHeight = 1.7f; // Hauteur de la cible (le perso)

    private float x = 0.0f; // Rotation en X
    private float y = 0.0f; // Rotation en Y
    private float currentDistance; // Distance actuelle de la camera
    private float desiredDistance; // Distance desiree de la camera
    private float correctedDistance; // Distance corrigee de la camera

    void Start()
    {
        // Recuperation des angles de rotation de la camera
        Vector3 angles = transform.eulerAngles;
        x = angles.y; // Rotation en Y
        y = angles.x;  // Rotation en X

        currentDistance = distance; // Distance actuelle de la camera egale a la distance
        desiredDistance = distance; // Distance desiree de la camera egale a la distance
        correctedDistance = distance; // Distance corrigee de la camera egale a la distance

        // Si il y a un rigidbody, on bloque la rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    void LateUpdate()
    {
        // Si il y a une cible
        if (!target)
            return; // On sort de la fonction

        x += Input.GetAxis("Mouse X") * xSpeed * 0.02f; // Rotation en X
        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f; // Rotation en Y

        y = ClampAngle(y, yMinLimit, yMaxLimit); // On limite la rotation en Y avec un Clamp()

        Quaternion rotation = Quaternion.Euler(y, x, 0); // On applique la rotation aux EulerAngles

        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomDampening * Mathf.Abs(desiredDistance); // On scroll pour zoomer
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance); // On limite la distance desiree
        correctedDistance = desiredDistance; // On applique la distance desiree a la distance corrigee

        Vector3 position = target.position - (rotation * Vector3.forward * desiredDistance); // On applique la position de la cible

        RaycastHit collisionHit; // On cree un raycast pour la collision de la camera

        Vector3 trueTargetPosition = target.position - new Vector3(0, -targetHeight, 0); // la position de la camera par rapport a la cible

        // On verifie si il y a une collision avec le lineCast provenant de la camera
        if (Physics.Linecast(trueTargetPosition, position, out collisionHit, collisionLayers))
        {
            correctedDistance = Vector3.Distance(trueTargetPosition, collisionHit.point); // On applique la distance corrigee pour eviter la collision
        }

        // On applique le dampening du zoom
        currentDistance = Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening);

        // On limite la distance actuelle
        position = target.position - (rotation * Vector3.forward * currentDistance + new Vector3(0, -targetHeight, 0));

        transform.rotation = rotation; // On applique la rotation
        transform.position = position; // On applique la position
    }

    // Fonction pour limiter l'angle de rotation
    public static float ClampAngle(float angle, float min, float max)
    {
        // Si l'angle est superieur a 360 ou inferieur a -360, on le remet entre -360 et 360
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}