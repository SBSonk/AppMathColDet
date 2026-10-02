using System.Collections;
using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] RectTransform coinTargetTransform;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] Transform coinContainer;

    [Header("Lerp & Animation Settings")]
    [SerializeField] Ease.EaseType easeType = Ease.EaseType.OutQuart;
    [SerializeField] float lerpDuration = 1.0f;
    [SerializeField] string textPrefix = "Coins: ";

    [Header("Camera & Canvas")]
    [SerializeField] Camera mainCamera;
    [SerializeField] Canvas canvas;

    int _displayedCoins;
    bool _isSubscribed;

    public Ease.EaseType EaseType
    {
        get => easeType;
        set => easeType = value;
    }

    public float LerpDuration
    {
        get => lerpDuration;
        set => lerpDuration = value;
    }

    public int DisplayedCoins => _displayedCoins;

    public void SetEase(Ease.EaseType newEase) => easeType = newEase;
    public void SetLerpDuration(float duration) => lerpDuration = duration;

    void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        if (coinContainer == null) coinContainer = transform;
        if (coinTargetTransform == null && coinsText != null) coinTargetTransform = coinsText.rectTransform;
    }

    void Start()
    {
        SubscribeToGameManager();

        if (GameManager.Instance != null)
        {
            _displayedCoins = GameManager.Instance.CurrentCoins;
        }
        UpdateCoinText();
    }

    void OnEnable()
    {
        SubscribeToGameManager();
    }

    void OnDisable()
    {
        UnsubscribeFromGameManager();
    }

    public void SubscribeToGameManager()
    {
        if (!_isSubscribed && GameManager.Instance != null)
        {
            GameManager.Instance.OnCoinsAdded += AddCoins;
            _isSubscribed = true;
        }
    }

    public void UnsubscribeFromGameManager()
    {
        if (_isSubscribed && GameManager.Instance != null)
        {
            GameManager.Instance.OnCoinsAdded -= AddCoins;
            _isSubscribed = false;
        }
    }

    public void SetCoinsValueInstant(int amount)
    {
        _displayedCoins = amount;
        UpdateCoinText();
    }

    public void AddCoins(int amount, Vector3 worldPosition)
    {
        if (amount <= 0) return;

        StartCoroutine(AnimateCoinRoutine(worldPosition, amount));
    }

    IEnumerator AnimateCoinRoutine(Vector3 worldPosition, int amount)
    {
        GameObject coinGO = null;
        if (coinPrefab != null)
        {
            coinGO = Instantiate(coinPrefab, coinContainer != null ? coinContainer : transform);
        }

        if (coinGO != null)
        {
            RectTransform coinRect = coinGO.GetComponent<RectTransform>();
            if (coinRect == null)
            {
                coinRect = coinGO.AddComponent<RectTransform>();
            }

            SetInitialPosition(coinRect, worldPosition);

            Vector3 startPos = coinRect.position;
            float elapsed = 0f;

            while (elapsed < lerpDuration)
            {
                elapsed += Time.deltaTime;
                float t = lerpDuration > 0f ? Mathf.Clamp01(elapsed / lerpDuration) : 1f;
                float easedT = Ease.EvaluateEaseType(easeType, t);

                Vector3 targetPos = coinTargetTransform != null
                    ? coinTargetTransform.position
                    : (coinsText != null ? coinsText.transform.position : transform.position);

                coinRect.position = Vector3.Lerp(startPos, targetPos, easedT);
                yield return null;
            }

            Destroy(coinGO);
        }

        _displayedCoins += amount;
        UpdateCoinText();
    }

    void SetInitialPosition(RectTransform rect, Vector3 worldPosition)
    {
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        Camera cam = mainCamera != null ? mainCamera : Camera.main;
        if (cam == null) cam = FindAnyObjectByType<Camera>();

        Vector3 screenPos = cam != null ? cam.WorldToScreenPoint(worldPosition) : worldPosition;

        rect.position = screenPos;
    }

    void UpdateCoinText()
    {
        if (coinsText != null)
        {
            coinsText.SetText($"{textPrefix}{_displayedCoins}");
        }
    }
}
