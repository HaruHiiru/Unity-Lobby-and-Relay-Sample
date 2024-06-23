using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

public class authManager : MonoBehaviour
{
    [SerializeField] private Button signInButton;
    [SerializeField] private Canvas signInCanvas;
    [SerializeField] private Canvas initialCanvas;
    [SerializeField] private TMP_InputField playerNameInputField;

    async void Start()
    {
        await UnityServices.InitializeAsync();
    }

    private void OnEnable()
    {
        signInButton.onClick.AddListener(SignIn);
    }

    private void OnDisable()
    {
        signInButton.onClick.RemoveAllListeners();
    }

    public async void SignIn()
    {

        if(playerNameInputField.text != "")
        {
            await signInAnonymous();
            signInCanvas.gameObject.SetActive(false);
            initialCanvas.gameObject.SetActive(true);
        }
        else
        {
            print("Enter a valid user name");
        }
            
    }

    async Task signInAnonymous()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            print("Signed in");
            print("Player ID: " + AuthenticationService.Instance.PlayerId);
            print("Player Name: " + playerNameInputField.text);
        }
        catch (AuthenticationException ex)
        {
            print("Sign in failed");
            Debug.LogException(ex);
        }
    }
}
