using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using UnityEngine;

public class GPGSManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public void GPGS_Login()
    {
        //PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }



    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
        }
        else
        {
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }
}
