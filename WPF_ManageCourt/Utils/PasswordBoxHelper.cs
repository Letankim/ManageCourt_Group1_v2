using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace WPF_ManageCourt.Utils
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BindablePasswordProperty =
            DependencyProperty.RegisterAttached("BindablePassword", typeof(string), typeof(PasswordBoxHelper),
                new PropertyMetadata(string.Empty, OnBindablePasswordChanged));

        public static string GetBindablePassword(DependencyObject obj)
        {
            return (string)obj.GetValue(BindablePasswordProperty);
        }

        public static void SetBindablePassword(DependencyObject obj, string value)
        {
            obj.SetValue(BindablePasswordProperty, value);
        }

        private static void OnBindablePasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                passwordBox.Password = e.NewValue as string;
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetBindablePassword(passwordBox, passwordBox.Password);
            }
        }
    }
}
