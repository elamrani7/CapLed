using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CapLed.Desktop.Models;
using CapLed.Desktop.Services;
using CapLed.Desktop.ViewModels.Base;

namespace CapLed.Desktop.ViewModels;

public class UserViewModel : BaseViewModel
{
    private readonly UserService _userService;
    private readonly IConfirmationService _confirmationService;

    // ─── Collections & Selection ─────────────────────────────────────────────
    public ObservableCollection<UserModel> Users { get; } = new();
    public ObservableCollection<string> Roles { get; } = new() { "ADMIN", "STOCK_MANAGER" };

    private UserModel? _selectedUser;
    public UserModel? SelectedUser
    {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);
    }

    // ─── Form Fields (Edit/Create) ───────────────────────────────────────────
    private string _fullName = string.Empty;
    public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }

    private string _email = string.Empty;
    public string Email { get => _email; set => SetProperty(ref _email, value); }

    private string _selectedRole = "STOCK_MANAGER";
    public string SelectedRole { get => _selectedRole; set => SetProperty(ref _selectedRole, value); }


    private string _password = string.Empty;
    public string Password { get => _password; set => SetProperty(ref _password, value); }

    private string _confirmPassword = string.Empty;
    public string ConfirmPassword { get => _confirmPassword; set => SetProperty(ref _confirmPassword, value); }

    private bool _isEditMode;
    public bool IsEditMode { get => _isEditMode; set => SetProperty(ref _isEditMode, value); }

    private int? _editingUserId;

    // ─── Commands ────────────────────────────────────────────────────────────
    public ICommand RefreshCommand { get; }
    public ICommand AddUserCommand { get; }
    public ICommand EditUserCommand { get; }
    public ICommand SaveUserCommand { get; }
    public ICommand DeleteUserCommand { get; }
    public ICommand ClearFormCommand { get; }

    // ─── Constructor ─────────────────────────────────────────────────────────
    public UserViewModel(UserService userService, IConfirmationService confirmationService)
    {
        _userService = userService;
        _confirmationService = confirmationService;

        RefreshCommand = new AsyncRelayCommand(LoadUsersAsync);
        AddUserCommand = new RelayCommand(PrepareForAdd);
        EditUserCommand = new RelayCommand(PrepareForEdit, () => SelectedUser != null);
        SaveUserCommand = new AsyncRelayCommand(SaveUserAsync, () => !IsSaving);
        DeleteUserCommand = new AsyncRelayCommand(DeleteUserAsync, () => SelectedUser != null && !IsSaving);
        ClearFormCommand = new RelayCommand(ClearForm);
    }

    // ─── Logic ───────────────────────────────────────────────────────────────
    public async Task InitializeAsync()
    {
        await LoadUsersAsync();
    }

    public async Task LoadUsersAsync()
    {
        ErrorMessage = null;
        SuccessMessage = null;
        BeginOperation();
        try
        {
            var result = await _userService.GetAllAsync();
            Users.Clear();
            if (result != null)
            {
                foreach (var u in result) Users.Add(u);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger les utilisateurs : " + ex.Message;
        }
        finally
        {
            EndOperation();
        }
    }

    private void PrepareForAdd()
    {
        ClearForm();
        IsEditMode = false;
        _editingUserId = null;
    }

    private void PrepareForEdit()
    {
        if (SelectedUser == null) return;

        IsEditMode = true;
        _editingUserId = SelectedUser.Id;
        FullName = SelectedUser.FullName;
        Email = SelectedUser.Email;
        SelectedRole = SelectedUser.Role;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        ErrorMessage = null;
        SuccessMessage = null;
    }

    private async Task SaveUserAsync()
    {
        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Le nom et l'email sont obligatoires.";
            return;
        }

        if (!IsEditMode)
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Le mot de passe est obligatoire pour un nouvel utilisateur.";
                return;
            }
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Les mots de passe ne correspondent pas.";
                return;
            }
        }

        BeginSave();

        try
        {
            bool success;
            if (IsEditMode && _editingUserId.HasValue)
            {
                var updateModel = new UserUpdateModel
                {
                    FullName = FullName,
                    Email = Email,
                    Role = SelectedRole,
                };
                success = await _userService.UpdateAsync(_editingUserId.Value, updateModel);
            }
            else
            {
                var createModel = new UserCreateModel
                {
                    FullName = FullName,
                    Email = Email,
                    Password = Password,
                    ConfirmPassword = ConfirmPassword,
                    Role = SelectedRole
                };
                success = await _userService.CreateAsync(createModel);
            }

            if (success)
            {
                SuccessMessage = IsEditMode ? "Utilisateur mis à jour." : "Utilisateur créé avec succès.";
                await LoadUsersAsync();
                ClearForm();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur lors de l'enregistrement : " + ex.Message;
        }
        finally
        {
            EndSave();
        }
    }

    private async Task DeleteUserAsync()
    {
        if (SelectedUser == null) return;

        if (SelectedUser.Email == CapLed.Desktop.Core.AppSession.Current.Email)
        {
            ErrorMessage = "Vous ne pouvez pas supprimer votre propre compte.";
            return;
        }

        if (!_confirmationService.Confirm("Suppression", $"Voulez-vous vraiment supprimer l'utilisateur '{SelectedUser.FullName}' ?"))
            return;

        BeginSave();
        try
        {
            bool success = await _userService.DeleteAsync(SelectedUser.Id);
            if (success)
            {
                SuccessMessage = "Utilisateur supprimé.";
                await LoadUsersAsync();
                ClearForm();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de supprimer l'utilisateur : " + ex.Message;
        }
        finally
        {
            EndSave();
        }
    }

    private void ClearForm()
    {
        FullName = string.Empty;
        Email = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        SelectedRole = "STOCK_MANAGER";
        IsEditMode = false;
        _editingUserId = null;
        ErrorMessage = null;
        SuccessMessage = null;
    }
}
