using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health System Configuration")]
    [SerializeField] int health;
    [SerializeField] int maxHealth;

    [Header("Feedback config")]
    [SerializeField] Material damagedMat; //Material de feedback de daño
    [SerializeField] MeshRenderer enemyRend; //Renderer del modelo
    [SerializeField] GameObject deathVfx;
    Material baseMat; //MAterial base del modelo

    private void Awake()
    {
        health = maxHealth;
        baseMat = enemyRend.material; //Se almacena el material base del modelo del enemigo
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            health = 0; //La vida no bajara de 0
            deathVfx.SetActive(true); //Encender vfx de muerte
            deathVfx.transform.position = transform.position; //Ponemos el vfx en la posicion actual del enemigo
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage; //Quita tanta vida como valor de daño viene de fuera
        enemyRend.material = damagedMat; //Se cambia de forma temporal el material base por el dañado
        Invoke(nameof(ResetEnemyMat), 0.1f); //Llama al reseteo del material con 0,1 segundos de espera
    }

    void ResetEnemyMat()
    {
        enemyRend.material = baseMat; //Cambia el material al material base
    }
}
