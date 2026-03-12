using UnityEngine;
using UnityEngine.AI;

public class EnemyAIBase : MonoBehaviour
{
    #region General Variables
    [Header("AI Configuration")]
    [SerializeField] NavMeshAgent agent; //Referencia al cerebro del agente
    [SerializeField] Transform target;
    [SerializeField] LayerMask targetLayer; 
    [SerializeField] LayerMask groundLayer; //Evita que el agente vaya a zonas que no tengan suelo

    [Header("Patroling Stats")]
    [SerializeField] float walkPointRange = 10f; //Radio maximo para determinar puntos a perseguir
    Vector3 walkPoint; //Posicion del punto random a perseguir
    bool walkPointSet; //Hay punto a perseguir generado? Si es falso generara uno


    [Header("Attacking Stats")]
    [SerializeField] float timeBetweenAttacks = 1f; //Cooldown entre ataques
    [SerializeField] GameObject projectile;
    [SerializeField] Transform shootPoint;
    [SerializeField] float shootSpeedY; //Fuerza de disparo hacia arriba (Catapulta)
    [SerializeField] float shootSpeedZ = 10f; //Fuerza hacia adelante (Tiene que estar siempre)
    bool alreadyAttacked;

    [Header("States & Detection")]
    [SerializeField] float sightRange = 8f; //Radio del detector de persecucion
    [SerializeField] float attackRange = 2f; //Radio del detector de ataque
    [SerializeField] bool targetInSightRange; //Determina si se puede perseguir al objetivo
    [SerializeField] bool targetInAttackRange; //Determina si se puede atacar al objetivo

    [Header("Stuck Detection")]
    [SerializeField] float stuckCheckTime = 2f; //Tiempo que el agente espera estando quieto antes de darse cuenta de que esta atascado
    [SerializeField] float stuckThreshold = 0.1f; //Margen de deteccion de estar atascado
    [SerializeField] float maxStuckDuration = 3f; //Tiempo maximo de estar atascado

    float stuckTimer; //Reloj que cuenta el tiempo de estar atascado
    float lastCheckTime;
    Vector3 lastPosition; //Posicion del ultimo punto perseguido

    #endregion

    private void Awake()
    {
        target = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;
        lastCheckTime = Time.time;
    }


    void Update()
    {
        EnemyStateUpdater();
    }

    void EnemyStateUpdater()
    {
        //Metodo encargado de gestionar el cambio de estados del enemigo
        //1 - Cambio de estado de los bools
        //Primero detectamos si ls targets estan en vision
        Collider[] hits = Physics.OverlapSphere(transform.position, sightRange, targetLayer);
        targetInSightRange = hits.Length > 0;
        //Segundo, si estan en vision detectamos si ademas estan en ataque
        if (targetInSightRange)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            targetInAttackRange = distance <= attackRange;
        }
        else targetInAttackRange = false;
        //2 - Cambio de estados segun booleanos
        if (!targetInSightRange && !targetInAttackRange)
        {
            Patroling();
        }
        else if (targetInSightRange && !targetInAttackRange)
        {
            ChaseTarget();
        }
        else if (targetInSightRange && targetInAttackRange)
        {
            AttackTarget();
        }
    }

    void Patroling()
    {
        Debug.Log("Enemigo en estado patrulla");
    }

    void ChaseTarget()
    {
        //accion que le dice al agente que persiga al objetivo
        agent.SetDestination(target.position);
    }

    void AttackTarget()
    {
        //Accion que contiene la logica de ataque
        //1 - EL agente se quedara quieto (Perseguirse a si mismo
        agent.SetDestination(transform.position);
        //2 - Aplicar una rotacion suavizada para que el agente mire al objetivo antes de atacar
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, agent.angularSpeed * Time.deltaTime);
        }

        //3 - Se ataca (Solo si no se esta atacando)

        if (!alreadyAttacked)
        {
            Rigidbody rb = Instantiate(projectile, shootPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * shootSpeedZ, ForceMode.Impulse);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return; //Si estamos jugando una build no se ejecutara el resto del codigo

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);


    }
}
