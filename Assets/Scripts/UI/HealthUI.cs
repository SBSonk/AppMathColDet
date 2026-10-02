using UnityEngine;
using UnityEngine.UI;
using Helpers;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Image healthBarGhost, healthBar;

    [Header("Feel")]
    [SerializeField] float damageHoldTime = 0.5f, ghostLerpTime = .5f;
    [SerializeField] float liveLerpSpeed = 25f;

    float _targetRatio, _ghostRatio, _ghostRatioStart, _liveRatio;

    float _holdTimer, _ghostTimer;

    void Start()
    {
        SetHealthValueInstant(1);
    }

    public void SetHealthValueInstant(float ratio)
    {
        _ghostRatio = ratio;
        _targetRatio = ratio;
        _liveRatio = ratio;
    }

    public void SetHealthValue(float ratio)
    {
        _ghostRatioStart = _ghostRatio;

        _targetRatio = ratio;
        _holdTimer = damageHoldTime;

        _ghostTimer = 0;
    }

    void Update()
    {
        if (_holdTimer > 0)
        {
            _holdTimer -= Time.deltaTime;
        } 
        else
        {
            _ghostTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(_ghostTimer / ghostLerpTime);
            _ghostRatio = Mathf.Lerp(_ghostRatioStart, _targetRatio, Ease.OutQuart(progress));
        }

        _liveRatio = Mathf.Lerp(_liveRatio, _targetRatio, Time.deltaTime * liveLerpSpeed);

        healthBar.fillAmount = _liveRatio;
        healthBarGhost.fillAmount = _ghostRatio;
    }
}
