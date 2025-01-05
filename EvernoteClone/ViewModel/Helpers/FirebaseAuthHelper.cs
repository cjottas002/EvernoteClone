using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EvernoteClone.Model;
using Newtonsoft.Json;

namespace EvernoteClone.ViewModel.Helpers;

public abstract class FirebaseAuthHelper
{
    private static string api_key = "";

    public static async Task<bool> RegisterAsync(User user)
    {
        using var httpClient = new HttpClient();
        var body = new
        {
            email = user.UserName,
            password = user.Password,
            returnSecureToken = true
        };

        var bodyJson = JsonConvert.SerializeObject(body);
        var data = new StringContent(bodyJson, Encoding.UTF8, "application/json");

        var response =
            await httpClient.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key{api_key}", data);

        if (response.IsSuccessStatusCode)
        {
            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<FirebaseResult>(resultJson);
            App.UserId = result.LocalId;
            return true;
        }

        var errorJson = await response.Content.ReadAsStringAsync();
        var error = JsonConvert.DeserializeObject<Error>(errorJson);
        MessageBox.Show(error.ErrorDetails.Message);
        return false;
    }
    
    
    public static async Task<bool> LoginAsync(User user)
    {
        using var httpClient = new HttpClient();
        var body = new
        {
            email = user.UserName,
            password = user.Password,
            returnSecureToken = true
        };

        var bodyJson = JsonConvert.SerializeObject(body);
        var data = new StringContent(bodyJson, Encoding.UTF8, "application/json");

        var response =
            await httpClient.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key{api_key}", data);

        if (response.IsSuccessStatusCode)
        {
            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<FirebaseResult>(resultJson);
            App.UserId = result.LocalId;
            return true;
        }

        var errorJson = await response.Content.ReadAsStringAsync();
        var error = JsonConvert.DeserializeObject<Error>(errorJson);
        MessageBox.Show(error.ErrorDetails.Message);
        return false;
    }
}

public class FirebaseResult
{
    public string Kind { get; set; }
    public string IdToken { get; set; }
    public string Email { get; set; }
    public string RefreshToken { get; set; }
    public string ExpiresIn { get; set; }
    public string LocalId { get; set; }
}

public abstract class ErrorDetails
{
    public int Code { get; set; }
    public string Message { get; set; }
}

public class Error
{
    public ErrorDetails ErrorDetails { get; set; }
}