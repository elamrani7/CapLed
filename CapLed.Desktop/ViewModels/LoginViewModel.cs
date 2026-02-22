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

    private bool _isLoggingIn;
    public bool IsLoggingIn
    {
        get => _isLoggingIn;
        set
        {
            if (SetProperty(ref _isLoggingIn, value))
            {
                OnPropertyChanged(nameof(IsLoading)); // Alias for BaseViewModel if needed
            }
        }
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel(AuthService authService, MainViewModel mainViewModel)
    {
        _authService = authService;
        _mainViewModel = mainViewModel;

        LoginCommand = new AsyncRelayCommand(ExecuteLoginAsync, CanExecuteLogin);
    }

    private bool CanExecuteLogin() => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password) && !IsLoggingIn;

    private async Task ExecuteLoginAsync()
    {
        ErrorMessage = null;
        IsLoggingIn = true;

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
        catch (Exception ex)
        {
            ErrorMessage = $"Une erreur est survenue : {ex.Message}";
        }
        finally
        {
            IsLoggingIn = false;
        }
    }
}
