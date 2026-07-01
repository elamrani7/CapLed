using CapLed.Desktop.Services;
using CapLed.Desktop.Core;
using CapLed.Desktop.ViewModels.Base;
using System.Windows.Input;

namespace CapLed.Desktop.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly AuthService _authService;
    private readonly MainViewModel _mainViewModel;

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }


    public ICommand LoginCommand { get; }

    public LoginViewModel(AuthService authService, MainViewModel mainViewModel)
    {
        _authService = authService;
        _mainViewModel = mainViewModel;

        LoginCommand = new AsyncRelayCommand(ExecuteLoginAsync, CanExecuteLogin);
    }

    private bool CanExecuteLogin() => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password) && !IsLoading;

    private async Task ExecuteLoginAsync()
    {
        ErrorMessage = string.Empty;
        BeginOperation();

        try
        {
            bool success = await _authService.LoginAsync(Email, Password);

            if (success)
            {
                // Navigate to dashboard or main shell
                _mainViewModel.ShowDashboardCommand.Execute(null);

                // Clear password for security
                Password = string.Empty;
            }
            else
            {
                ErrorMessage = "Email ou mot de passe incorrect.";
            }
        }
        catch (ApiException ex)
        {
            // Real API error (401 wrong credentials, network failure, etc.)
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            // Unexpected failure fallback
            ErrorMessage = $"Une erreur inattendue est survenue : {ex.Message}";
        }
        finally
        {
            EndOperation();
        }
    }
}
