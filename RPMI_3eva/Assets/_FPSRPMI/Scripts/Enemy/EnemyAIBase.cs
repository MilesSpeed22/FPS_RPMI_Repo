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

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
