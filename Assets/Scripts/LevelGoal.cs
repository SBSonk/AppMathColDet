using UnityEngine;

public class LevelGoal : TriggerZone
{
    [SerializeField] GameObject winUI;

    bool _hasWon;

    protected override void Awake()
    {
        base.Awake();

        if (winUI != null)
        {
            winUI.SetActive(false);
        }
    }

    protected override void OnPlayerEnter()
    {
        base.OnPlayerEnter();
        OnGoalReached();
    }

    public virtual void OnGoalReached()
    {
        if (_hasWon) return;
        _hasWon = true;

        BaseTurret.StopAllTurrets();

        // Display Win UI
        if (winUI != null)
        {
            winUI.SetActive(true);
        }
    }
}
