using System.Windows;
using System.Windows.Controls;

namespace CapLed.Desktop.Views.Users;

/// <summary>
/// A helper class to allow two-way binding of a PasswordBox's Password property.
/// Standard PasswordBox.Password is not a DependencyProperty for security reasons,
/// but in a LOB application back-office, this helper is commonly used for convenience.
/// </summary>
public static class PasswordBoxHelper
{
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

    public static readonly DependencyProperty AttachProperty =
        DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordBoxHelper),
            new PropertyMetadata(false, OnAttachChanged));

    private static readonly DependencyProperty IsUpdatingProperty =
        DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordBoxHelper));

    public static void SetPassword(DependencyObject dp, string value) => dp.SetValue(PasswordProperty, value);
    public static string GetPassword(DependencyObject dp) => (string)dp.GetValue(PasswordProperty);

    public static void SetAttach(DependencyObject dp, bool value) => dp.SetValue(AttachProperty, value);
    public static bool GetAttach(DependencyObject dp) => (bool)dp.GetValue(AttachProperty);

    private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
            if (!(bool)passwordBox.GetValue(IsUpdatingProperty))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }
    }

    private static void OnAttachChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            if ((bool)e.OldValue) passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
            if ((bool)e.NewValue) passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }
    }

    private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            passwordBox.SetValue(IsUpdatingProperty, true);
            SetPassword(passwordBox, passwordBox.Password);
            passwordBox.SetValue(IsUpdatingProperty, false);
        }
    }
}
