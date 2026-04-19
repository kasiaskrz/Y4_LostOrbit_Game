using UnityEngine;
using UnityEngine.AI;

public class EnemyTutorialTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyTutorialPanel;
    public EnemyHealth enemyHealth;
    public GameObject hintPanel;

    [Header("Slow Mo")]
    public float slowMoScale = 0.3f;

    private bool hasTriggered = false;
    private bool enemyWasDead = false;

    void Update()
    {
        if (!hasTriggered) return;
        if (enemyHealth != null && enemyHealth.IsDead && !enemyWasDead)
        {
            enemyWasDead = true;
            EndTutorial();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;
        hasTriggered = true;
        Time.timeScale = slowMoScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        EnemyWeaponAttack weapon = enemyHealth.GetComponent<EnemyWeaponAttack>();
        if (weapon != null) weapon.tutorialLocked = false;
        EnemyBehavior behavior = enemyHealth.GetComponent<EnemyBehavior>();
        if (behavior != null) behavior.Unlock();
        if (hintPanel != null) hintPanel.SetActive(false);
        if (enemyTutorialPanel != null) enemyTutorialPanel.SetActive(true);
    }

    void EndTutorial()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        if (enemyTutorialPanel != null) enemyTutorialPanel.SetActive(false);
        if (hintPanel != null) hintPanel.SetActive(true);
    }
}