using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    #region General Variables
    [Header("Movement & Look")]
    [SerializeField] GameObject camHolder;
    [SerializeField] float speed = 5f;
    [SerializeField] float sprintSpeed = 8f;
    [SerializeField] float crouchSpeed = 3f;
    [SerializeField] float maxForce = 1f; //Fuerza maxima de aceleracion
    [SerializeField] float sensitivity = 0.1f; //Camara sensibilidad

    [Header("Jump & GroundCheck")]
    [SerializeField] float jumpForce = 5f;
    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.3f;
    [SerializeField] LayerMask groundLayer;


    [Header("Player State Bools")]
    [SerializeField] bool isSprinting;
    [SerializeField] bool isCrouching;
    #endregion
    //Variables de referencia privadas
    Rigidbody rb;
    Animator anim;

    //Variables para el input
    Vector2 moveInput;
    Vector2 lookInput;
    float lookRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        //Bloquear el cursor del raton
        Cursor.lockState = CursorLockMode.Locked; //Mueve el cursor al centro y lo deja ahi
        Cursor.visible = false; //Oculta el cursor
    }

    void Update()
    {
        //Groundcheck
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        //Dibujar un rayo ficticio en escena para determinar la orientacion de la camara
        Debug.DrawRay(camHolder.transform.position, camHolder.transform.forward * 100f, Color.red);

    }

    private void FixedUpdate()
    {
        Movement();   
    }
    private void LateUpdate()
    {
        CameraLook();
    }

    void CameraLook()
    {
        //Rotacion horizontal del cuerpo del personaje
        transform.Rotate(Vector3.up * lookInput.x * sensitivity);
        //Rotacion vertical (la lleva la camara)
        lookRotation += (-lookInput.y * sensitivity);
        lookRotation = Mathf.Clamp(lookRotation, -90, 90);
        camHolder.transform.localEulerAngles = new Vector3(lookRotation, 0f, 0f);
    }

    void Movement()
    {
        Vector3 currentVelocity = rb.linearVelocity; //Calcular la velocidad actual del RigidBody constantemente
        Vector3 targetVelocity = new Vector3(moveInput.x, 0f, moveInput.y); //Velocidad a alcanzar que es igual a la direccion que pulsamos
        targetVelocity *= isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : speed); //Parentesis para que le de prioridad al crouch
        
        //Convertit direccion global en local
        targetVelocity = transform.TransformDirection(targetVelocity);

        //Calcular el cambio de velocidad(Aceleracion)
        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3(velocityChange.x, 0f, velocityChange.z);
        velocityChange = Vector3.ClampMagnitude(velocityChange, maxForce);

        //Aplicar la fuerza de movimiento/aceleracion
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void Jump()
    {
        if (isGrounded) rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    #region Input Methods
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) Jump();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        { 
            isCrouching = !isCrouching;
            anim.SetBool("isCrouching", isCrouching);
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed && !isCrouching) isSprinting = true;
        if (context.canceled) isSprinting = false;
    }

    #endregion
}
