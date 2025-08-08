using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AuthManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject loginPanel;
    public GameObject signupPanel;
    
    [Header("Login UI")]
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;
    public Button loginButton;
    public Button switchToSignupButton;
    public TextMeshProUGUI loginStatusText;
    
    [Header("Signup UI")]
    public TMP_InputField signupEmailInput;
    public TMP_InputField signupPasswordInput;
    public TMP_InputField confirmPasswordInput;
    public Button signupButton;
    public Button switchToLoginButton;
    public TextMeshProUGUI signupStatusText;
    
    [Header("Settings")]
    public string mainMenuSceneName = "MainMenu";
    
    private FirebaseAuth auth;
    private FirebaseUser user;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        SetupUI();
        ShowLoginPanel();
    }
    
    void SetupUI()
    {
        if (loginButton != null)
            loginButton.onClick.AddListener(() => LogIn(loginEmailInput.text, loginPasswordInput.text));
            
        if (switchToSignupButton != null)
            switchToSignupButton.onClick.AddListener(ShowSignupPanel);
        
        if (signupButton != null)
            signupButton.onClick.AddListener(() => SignUp(signupEmailInput.text, signupPasswordInput.text));
            
        if (switchToLoginButton != null)
            switchToLoginButton.onClick.AddListener(ShowLoginPanel);
    }

    public void SignUp(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowStatus("Please enter email and password", signupStatusText, true);
            return;
        }
        
        if (confirmPasswordInput != null && password != confirmPasswordInput.text)
        {
            ShowStatus("Passwords do not match", signupStatusText, true);
            return;
        }
        
        ShowStatus("Creating account...", signupStatusText, false);
        
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("SignUp Failed: " + task.Exception);
                ShowStatus("Sign up failed: " + GetErrorMessage(task.Exception), signupStatusText, true);
                return;
            }

            user = task.Result.User;
            Debug.Log("User signed up: " + user.Email);
            ShowStatus("Account created successfully!", signupStatusText, false);
            
            Invoke(nameof(GoToMainMenu), 1f);
        });
    }

    public void LogIn(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowStatus("Please enter email and password", loginStatusText, true);
            return;
        }

        ShowStatus("Signing in...", loginStatusText, false);
        Debug.Log("[AuthManager] Starting login for: " + email);

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("[AuthManager] SignIn Failed: " + task.Exception);
                ShowStatus("Sign in failed: " + GetErrorMessage(task.Exception), loginStatusText, true);
                return;
            }

            user = task.Result.User;
            Debug.Log("[AuthManager] User signed in successfully: " + user.Email);
            ShowStatus("Sign in successful!", loginStatusText, false);
            GoToMainMenu();
        });
    }
    
    void ShowStatus(string message, TextMeshProUGUI statusText, bool isError)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = isError ? Color.red : Color.green;
        }
    }
    
    string GetErrorMessage(System.AggregateException exception)
    {
        if (exception?.InnerException != null)
        {
            return exception.InnerException.Message;
        }
        return "Unknown error occurred";
    }
    
    public void ShowLoginPanel()
    {
        if (loginPanel != null) loginPanel.SetActive(true);
        if (signupPanel != null) signupPanel.SetActive(false);
        ClearStatus();
    }

    public void ShowSignupPanel()
    {
        if (signupPanel != null) signupPanel.SetActive(true);
        if (loginPanel != null) loginPanel.SetActive(false);
        ClearStatus();
    }
    
    void ClearStatus()
    {
        if (loginStatusText != null) loginStatusText.text = "";
        if (signupStatusText != null) signupStatusText.text = "";
    }
    
    void GoToMainMenu()
    {
        bool sceneFound = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log("[AuthManager] Build scene " + i + ": '" + sceneName + "'");
            
            if (sceneName.Equals(mainMenuSceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("[AuthManager] Found matching scene at index: " + i);
                sceneFound = true;
                break;
            }
        }
        
        if (!sceneFound)
        {
            Debug.LogError("[AuthManager] Scene '" + mainMenuSceneName + "' NOT FOUND in build settings!");
            ShowStatus("Error: Scene not found", loginStatusText, true);
            return;
        }
        
        try 
        {
            Debug.Log("[AuthManager] Attempting to load scene: " + mainMenuSceneName);
            SceneManager.LoadScene(mainMenuSceneName);
            Debug.Log("[AuthManager] SceneManager.LoadScene() called successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[AuthManager] Exception loading scene: " + e.Message);
            Debug.LogError("[AuthManager] Stack trace: " + e.StackTrace);
            ShowStatus("Error loading menu", loginStatusText, true);
        }
    }
}
