using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunSystem : MonoBehaviour
{
    #region General Variables
    [Header("General References")]
    [SerializeField] Camera fpsCam; //Referencia si disparamos desde el centro de la camara
    [SerializeField] Transform shootPoint; //Referencia si queremos disparar desde la punta del cañon
    [SerializeField] LayerMask impactLayer; //Capa con la que el Raycast interactua
    RaycastHit hit; //Almacen de la informacion de los objetos a los que el raycast puede impactar

    [Header("Weapon Parameters")]
    [SerializeField] int damage = 10; //Daño del arma por bala
    [SerializeField] float range = 100f; //Rango al que llega el arma 
    [SerializeField] float spread = 0f; //Radio de dispersion del arma
    [SerializeField] float shootingCooldown = 0.2f; //Tiempo entre disparos
    [SerializeField] float reloadTime = 1.5f; //Tiempo de recarga (Segundos)
    [SerializeField] bool allowButtonHold = false; //Si el disparo se ejecuta por clic (False) o por mantener clic (True)

    [Header("Bullet Management")]
    [SerializeField] int ammoSize = 30; //Cantidad maxima de balas por cargador
    [SerializeField] int bulletsPerTap = 1; //Cantidad de balas disparadas cada vez que disparamos
    int bulletsLeft; //Cantidad de balas en el cargador actualmente

    [Header("Feedback references")]
    [SerializeField] GameObject impactEffect; //Referencia al VFX de impacto de la bala

    [Header("Dev - Gun State Bools")]
    [SerializeField] bool shooting; //Indica si estamos disparando
    [SerializeField] bool canShoot; //Indica si podemos disparar en determinado momento del juego
    [SerializeField] bool reloading; //Indica si esta recargando el arma
    #endregion

    private void Awake()
    {
        bulletsLeft = ammoSize; //Al inicio de la partida el cargador esta lleno
        canShoot = true;
    }

    void Update()
    {
        //Condicion estricta de llamar a la rutina de disparo
        if (canShoot && shooting && !reloading && bulletsLeft > 0) StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        //Corrutina que se encarga de medir el tiempo entre disparos y la gestion del gasto de balas, llama al raycast de disparo
        canShoot = false;
        if (!allowButtonHold) shooting = false; //Cerrar el bucle de disparo por pulsacion
        for (int i = 0; i < bulletsPerTap; i++)
        {
            if (bulletsLeft <= 0) break; //break anula el bucle
            Shoot();
            bulletsLeft--; //resta 1 a la cantidad de balas actual
        }

        yield return new WaitForSeconds(shootingCooldown);
        canShoot = true;
    }

    void Shoot()
    {
        Vector3 direction = fpsCam.transform.forward; //Se lanza un rayo hacia delante de la camara
        //Añadir dispersion aleatoria segun el valor de spread
        direction.x += Random.Range(-spread, spread);
        direction.y += Random.Range(-spread, spread);

        //Anatomia Raycast: Physics.Raycast(Origen del rayo, direccion, almacen de la info del impacto, longitud del rayo, capa con la que impacta el rayo)
        if (Physics.Raycast(fpsCam.transform.position, direction, out hit, range, impactLayer))
        {
            Debug.Log(hit.collider.name);
        }
    }

    void Reload()
    {
        if (bulletsLeft < ammoSize && !reloading) StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        reloading = true;
        //Animacion
        yield return new WaitForSeconds(reloadTime);
        bulletsLeft = ammoSize; //Cantidad de balas actuales se iguala a la cantidad de balas maxima
        reloading = false;
    }

    #region Input methods

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (allowButtonHold)
        {
            shooting = context.ReadValueAsButton(); //Detecta constantemente si el boton de disparo esya apretado
        }
        else
        {
            if (context.performed) shooting = true; //shooting solo es verdadero por pulsacion
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed) Reload();
    }

    #endregion
}
