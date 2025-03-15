using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Database;

public class registerSceneInputFunctions : MonoBehaviour
{
    // variables used for inputs
    public TMP_InputField emailAddressInput;
    public TMP_InputField verificationCodeInput;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    // variable to see if a correct input was entered
    private bool isValidEmailAddress = false;
    private bool isValidVerificationCodeInput = false;
    private bool isValidUsername = false;
    private bool isValidPassword = false;

    // string version of the inputs
    private string emailAddress;
    private string verificationCode;
    private string verificationCodeAttempt;
    private string username;
    private string password;

    // variables used for the error message
    public TMP_Text errorMessage;

    // variables for fire base database
    public DependencyStatus dependencyStatus;
    public FirebaseAuth firebaseAuth;
    public FirebaseUser firebaseUser;
    public DatabaseReference firebaseDatabaseRef;


    private void Start()
    {
        verificationCode = generateVerificationCode();
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If dependencies are avalible, then the firebase database and authentication is initalised
                firebaseAuth = FirebaseAuth.DefaultInstance;
                firebaseDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                errorMessage.text = "Could not resolve all Firebase dependencies: " + dependencyStatus +
                "\n please restart to solve this problem, or solve the dependencies issues manually";
            }
        });
    }

    public async void registerButton()
    {
        if (enterValidEmailAddress())
        {
            isValidEmailAddress = true;
            StartCoroutine(enterVerificationCode());
        }
        // ensures that all the information is in the required format
        if (isValidEmailAddress && isValidUsername && isValidVerificationCodeInput && isValidPassword)
        {
            await registerAccount();
            SceneManager.LoadScene("mainMenu");
        }
    }

    private bool enterValidEmailAddress()
    {
        emailAddress = emailAddressInput.text;
        if (!emailAddress.Contains("@"))
        {
            errorMessage.text = "Invalid email address entered";
            return false;
        }
        else
        {
            errorMessage.text = "Sent verification code";
            generateVerificationEmail();
            return true;
        }
    }

    private IEnumerator enterVerificationCode()
    {
        errorMessage.text = "Please enter the verification code sent to your email.";

        // Waits for the user to input the verification code
        while (string.IsNullOrEmpty(verificationCodeInput.text) || verificationCodeInput.text.Length < verificationCode.Length)
        {
            yield return null;
        }

        verificationCodeAttempt = verificationCodeInput.text;
        if (verificationCodeAttempt == verificationCode)
        {
            // Verification successful, proceed with registration
            isValidVerificationCodeInput = true;
            enterValidUsername();
            enterValidPassword();
        }
        else
        {
            errorMessage.text = "Invalid verification code entered";
        }
    }

    private async void enterValidUsername()
    {
        username = usernameInput.text;

        if (string.IsNullOrEmpty(username))
        {
            errorMessage.text = "Username is required";
            return;
        }

        // Check if the username already exists in the database
        bool usernameExists = await checkUsernameExists(username);
        if (usernameExists)
        {
            errorMessage.text = "Username is already taken, please choose another one";
            return;
        }

        isValidUsername = true;
    }

    private async Task<bool> checkUsernameExists(string username)
    {
        // creates a reference to all the users in the database
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("users");

        // Gets all the data stored in the database and stores it in snapshot
        DataSnapshot snapshot = await reference.GetValueAsync();

        // loops through each child node in the data
        foreach (DataSnapshot childSnapshot in snapshot.Children)
        {
            // compares inputted username and the usernames stored in the database
            string storedUsername = childSnapshot.Child("username").Value.ToString();

            if (storedUsername == username)
            {
                return true; // Username exists
            }
        }
        return false;
    }

    private void enterValidPassword()
    {
        
        password = passwordInput.text;
        bool hasCapitalLetter = false;
        bool hasSymbol = false;
        bool hasNumber = false;


        if (password.Length < 7)
        {
            errorMessage.text = "Password is too short, try again";
        }
        foreach (var c in password)
        {
            // check ASCII for capital letters
            if (c >= 65 && c <= 90)
            {
                hasCapitalLetter = true;
            }
            // check ASCII for symbols
            else if ((c >= 33 && c <= 47) || (c >= 58 && c <= 64) || (c <= 91 && c >= 96) || (c <= 123 && c >= 126))
            {
                hasSymbol = true;
            }
            // check ASCII for numbers
            else if (c >= 48 && c <= 57)
            {
                hasNumber = true;
            }
        }
        if (!(hasCapitalLetter && hasSymbol && hasNumber)) // checks number, cap and symbol requirement
        {
            errorMessage.text = "Password does not meet the requirements of a capital letter, a symbol and a number";
        }
        else
        {
            isValidPassword = true;
        }
    }

    private void generateVerificationEmail()
    {
        // information of the email account that sends the verification email
        string senderEmail = "";    // removed for privacy
        string senderPassword = ""; // removed for privacy

        // constructing the email
        MailMessage verificationEmail = new MailMessage();
        verificationEmail.Subject = "Verification email for the account: " + emailAddress;
        verificationEmail.Body = "This is the verification code: " + verificationCode + " for the email address: " + emailAddress;

        // used to allow for secure sending of the verification email (gmail only)
        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
        smtpClient.Port = 587;
        smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
        smtpClient.EnableSsl = true;
        smtpClient.Send(senderEmail, emailAddress, verificationEmail.Subject, verificationEmail.Body);
    }

    private string generateVerificationCode()
    {
        string verificationCode = string.Empty;
        // have a string with all capital, non capital letters and numbers so it can be random
        // removed L, I, O and 0 because they look too similar in the email
        string characters = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz123456789";
        System.Random rand = new System.Random();
        int randomCharPointer = rand.Next(0, 57);
        for (int i = 0; i < 6; i++)
        {
            verificationCode += characters[randomCharPointer];
            randomCharPointer = rand.Next(0, 57);
        }
        return verificationCode;
    }
    private async Task registerAccount()
    {
        try
        {
            // Create the user with email and password in the authResult firebase
            AuthResult authResult = await firebaseAuth.CreateUserWithEmailAndPasswordAsync(emailAddress, password);
            FirebaseUser newUser = authResult.User;

            // Update user profile
            UserProfile profile = new UserProfile { DisplayName = username };
            await newUser.UpdateUserProfileAsync(profile);

            // Save user data to Firebase Realtime Database
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(newUser.UserId);
            Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "username", username },
            { "email", newUser.Email }
        };
            await reference.SetValueAsync(userData);
            errorMessage.text = "User data saved to Firebase Realtime Database.";
        }
        catch (Exception ex)
        {
            errorMessage.text = "Failed to register user" + ex;
        }
    }

}
