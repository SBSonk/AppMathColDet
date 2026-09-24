using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZone : TriggerZone
{
    public float explosionTime = 3f;
    float _timeInZone;

    protected override void OnPlayerInside()
    {
        base.OnPlayerInside();

        _timeInZone += Time.deltaTime;

        if (_timeInZone >= explosionTime)
        {
            KillPlayer();
        }
    }

    protected override void OnPlayerWarning()
    {
        base.OnPlayerWarning();
        _timeInZone = 0f;
    }

    protected override void OnPlayerOutside()
    {
        base.OnPlayerOutside();
        _timeInZone = 0f;
    }

    protected virtual void KillPlayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
