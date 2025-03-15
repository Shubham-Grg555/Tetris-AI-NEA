using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Database;

public class loginSceneInputFunctions : MonoBehaviour
{
    // TestTest123!
    // variables for input
    public TMP_InputField usernameOrEmailInput;
    public TMP_InputField passwordInput;

    // string version of the inputs
    private string usernameOrEmail;
    private string password;

    // variables used for the error message
    public TMP_Text errorMessage;

    // variables for fire base database
    public DependencyStatus dependencyStatus;
    public FirebaseAuth firebaseAuth;
    public FirebaseUser firebaseUser;
    public DatabaseReference firebaseDatabaseRef;

    void Start()
    {
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
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }


    public async void login()
    {
        usernameOrEmail = usernameOrEmailInput.text;
        bool usernameExists = await CheckUsernameExists(usernameOrEmail);
        bool emailAddressExists = await CheckIfEmailExists(usernameOrEmail);
        if (!usernameExists && !emailAddressExists)
        {
            errorMessage.text = "Username or email does not exist";
            return;
        }
        else
        {
            password = passwordInput.text;
            if (usernameExists)
            {
                checkPasswordAndUsername(usernameOrEmail, password);
            }
            else
            {
                checkPasswordAndEmail(usernameOrEmail);
            }
        }
    }

    private async Task<bool> CheckUsernameExists(string username)
    {
        // creates a reference to all the users in the database
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("users");

        // Gets all the data stored in the database and stores it in the snapshot
        DataSnapshot snapshot = await reference.GetValueAsync();

        // loops through each child node in the data
        foreach (DataSnapshot childSnapshot in snapshot.Children)
        {
            // // compares inputted username and the usernames stored in the database
            string storedUsername = childSnapshot.Child("username").Value.ToString();
            if (storedUsername == username)
            {
                return true; // Username exists
            }
        }
        return false;
    }

    private async Task<bool> CheckIfEmailExists(string email)
    {

        // creates a reference to all the users in the database
        DatabaseReference reference = firebaseDatabaseRef.Child("users");

        // Gets all the user data in the data base and stores it in the snapshot
        DataSnapshot snapshot = await reference.GetValueAsync();

        // Loop through each child node in the snapshot
        foreach (DataSnapshot childSnapshot in snapshot.Children)
        {
            // Gets the email address
            string storedEmail = childSnapshot.Child("email").Value.ToString();

            // Check if the inputted email address is the same as the email addresses stored in the database
            if (storedEmail == email)
            {
                return true; // email address exists
            }
        }
        return false;
    }

    private async void checkPasswordAndEmail(string email)
    {
        // attempts to sign in / authorise into the account
        AuthResult authResult = await firebaseAuth.SignInWithEmailAndPasswordAsync(email, password);
        if (authResult != null)
        {
            errorMessage.text = "sign in successful";
            SceneManager.LoadScene("mainMenu");
        }
        else
        {
            errorMessage.text = "Either email or password does not exist together as an account";
            return;
        }
    }

    public async void checkPasswordAndUsername(string username, string password)
    {
        // gets the user's email address via the username
        string email = await getEmailFromUsername(username);
        if (email != null)
        {
            // Signs in the user with the retrieved email and inputted password
            var authResult = await firebaseAuth.SignInWithEmailAndPasswordAsync(email, password);

            if (authResult != null)
            {
                errorMessage.text = "sign in successful";
                SceneManager.LoadScene("mainMenu");
            }
            errorMessage.text = "Either username or password does not exist together as an account";
        }
    }

    private async Task<string> getEmailFromUsername(string username)
    {
        // creates a reference to all the users in the database
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("users");

        // Gets all the data in the database and stores it in the snapshot
        DataSnapshot snapshot = await reference.GetValueAsync();

        // Loop through each child node to find the matching username and email address
        foreach (DataSnapshot childSnapshot in snapshot.Children)
        {
            // Get the username stored in the database
            string storedUsername = childSnapshot.Child("username").Value.ToString();

            // If the stored username matches the provided username
            if (storedUsername == username)
            {
                // Return the corresponding email address
                return childSnapshot.Child("email").Value.ToString();
            }
        }
        return null;
    }
}

