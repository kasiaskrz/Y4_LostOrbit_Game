using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;

public class SupabaseAuth : MonoBehaviour
{
    [Header("Supabase Config")]
    public string supabaseUrl = "https://ywuyfgvtazgysxmnvknv.supabase.co";
    public string anonKey = "YOUR_ANON_KEY";

    [Header("Login UI")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text statusText;

    [Header("Scene Flow")]
    public string gameSceneName = "GameScene";

    [Header("Session")]
    public string accessToken;
    public string userId;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // ---------- LOGIN ----------
    public void LoginFromUI()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            if (statusText != null) statusText.text = "Enter email and password.";
            return;
        }

        if (statusText != null) statusText.text = "Logging in...";

        StartCoroutine(LoginAndGoCoroutine(email, password));
    }

    IEnumerator LoginAndGoCoroutine(string email, string password)
    {
        yield return LoginCoroutine(email, password);

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(userId))
        {
            yield break;
        }

        if (statusText != null) statusText.text = "Success!";
        Loader.Load(Loader.Scene.SC001);
    }

    IEnumerator LoginCoroutine(string email, string password)
    {
        string url = $"{supabaseUrl}/auth/v1/token?grant_type=password";

        string jsonBody = JsonUtility.ToJson(new LoginBody
        {
            email = email,
            password = password
        });

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("apikey", anonKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Login failed: " + request.error);
            Debug.LogError(request.downloadHandler.text);

            if (statusText != null)
            {
                statusText.text = "Invalid email or password. Please try again.";
            }
            yield break;
        }

        var response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

        accessToken = response.access_token;
        userId = response.user.id;

        Debug.Log("Logged in as user: " + userId);
    }

    // ---------- SUBMIT RUN ----------
    public void SubmitRun(int levelNumber, int timeMs)
    {
        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(userId))
        {
            Debug.Log("Guest mode: run not submitted.");
            return;
        }

        StartCoroutine(SubmitRunCoroutine(levelNumber, timeMs));
    }

    IEnumerator SubmitRunCoroutine(int levelNumber, int timeMs)
    {
        string url = $"{supabaseUrl}/rest/v1/level_runs";

        string jsonBody = JsonUtility.ToJson(new RunBody
        {
            user_id = userId,
            level_number = levelNumber,
            time_ms = timeMs
        });

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("apikey", anonKey);
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        request.SetRequestHeader("Prefer", "return=minimal");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("SubmitRun failed: " + request.error);
            Debug.LogError(request.downloadHandler.text);
            yield break;
        }

        Debug.Log($"Run submitted! Level {levelNumber}, Time {timeMs}ms");
    }

    // ---------- JSON TYPES ----------
    [System.Serializable]
    class LoginBody
    {
        public string email;
        public string password;
    }

    [System.Serializable]
    class LoginResponse
    {
        public string access_token;
        public User user;
    }

    [System.Serializable]
    class User
    {
        public string id;
    }

    [System.Serializable]
    class RunBody
    {
        public string user_id;
        public int level_number;
        public int time_ms;
    }
}