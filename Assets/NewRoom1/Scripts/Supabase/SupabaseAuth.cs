using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;

public class SupabaseAuth : MonoBehaviour
{
    [Header("Supabase Config")]
    // NOTE: Make sure this URL matches your Supabase project exactly.
    public string supabaseUrl = "https://ywuyfgvtazgysxmnvknv.supabase.co";
    public string anonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inl3dXlmZ3Z0YXpxeXN4bm12bmt2Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjI1MzQyMzAsImV4cCI6MjA3ODExMDIzMH0.qOpekOPmKweh29QgtQUCGM-fAXPJZ58R0ccSjMET-rM";

    [Header("Login (fallback if not using UI inputs)")]
    public string email;
    public string password;

    [Header("Login UI (optional)")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text statusText;

    [Header("Scene Flow")]
    public string gameSceneName = "GameScene";

    [Header("Session (auto-filled after login)")]
    public string accessToken;
    public string userId;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // ---------- LOGIN ----------
    // Old method still works (uses the inspector email/password fields)
    public void Login()
    {
        StartCoroutine(LoginCoroutine());
    }

    // Use this from your Login button in the LoginScene UI
    public void LoginFromUI()
    {
        if (emailInput != null) email = emailInput.text.Trim();
        if (passwordInput != null) password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            if (statusText != null) statusText.text = "Enter email and password.";
            return;
        }

        if (statusText != null) statusText.text = "Logging in...";
        StartCoroutine(LoginAndGoCoroutine());
    }

    IEnumerator LoginAndGoCoroutine()
    {
        // Run the normal login flow
        yield return LoginCoroutine();

        // If login failed, do NOT change scenes
        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(userId))
        {
            if (statusText != null) statusText.text = "Login failed. Check details.";
            yield break;
        }

        if (statusText != null) statusText.text = "Success!";
        SceneManager.LoadScene(gameSceneName);
    }

    IEnumerator LoginCoroutine()
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
            if (statusText != null) statusText.text = "Login failed.";
            yield break;
        }

        var response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

        accessToken = response.access_token;
        userId = response.user.id;

        Debug.Log("Logged in as user: " + userId);
    }

    // ---------- SUBMIT RUN ----------
    public void SubmitTestRun()
    {
        // Test run: Level 1, 75.3 seconds
        SubmitRun(1, 75300);
    }

    public void SubmitRun(int levelNumber, int timeMs)
    {
        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(userId))
        {
            Debug.LogError("SubmitRun: Not logged in. Login first.");
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
