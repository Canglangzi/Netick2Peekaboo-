using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text progressText;
    [SerializeField] private Text statusText;
    [SerializeField] private Image loadingIcon;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("设置")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float rotationSpeed = 180f;
    
    private bool _isActive;
    private AsyncOperationHandle<GameObject> _loadingScreenHandle;
    private static LoadingScreen _instance;

    public static LoadingScreen Instance
    {
        get
        {
            if (_instance == null)
            {

            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        canvasGroup.alpha = 0;
       // gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isActive && loadingIcon.gameObject.activeSelf)
        {
            loadingIcon.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    public async UniTask Show()
    {
        if (_isActive) return;
        
        _isActive = true;
        gameObject.SetActive(true);
        
        // 重置状态
        SetProgress(0);
        SetStatus("正在初始化...");
        ShowLoadingIcon(true);
        
        // 淡入效果
        await Fade(0, 1);
    }

    public async UniTask Hide()
    {
        if (!_isActive) return;
        
        // 淡出效果
        await Fade(1, 0);
        
        // 重置状态
     //   gameObject.SetActive(false);
        _isActive = false;
    }

    private async UniTask Fade(float from, float to)
    {
        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, time / fadeDuration);
            await UniTask.Yield();
        }
    }

    public void SetProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);
        progressBar.value = progress;
        progressText.text = $"{progress * 100:F1}%";
    }

    public void SetStatus(string message)
    {
        statusText.text = message;
    }

    public void ShowLoadingIcon(bool show)
    {
        loadingIcon.gameObject.SetActive(show);
    }
}