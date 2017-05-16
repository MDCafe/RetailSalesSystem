using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace RetailManagementSystem.View.Behaviours
{
    class ClearOnFocusedBehavior : Behavior<System.Windows.Controls.TextBox>
    {
        private readonly RoutedEventHandler OnFocusHandler = (o, e) =>
        {
            var txtBox = o as TextBox;
            if (txtBox.Text == "0.0")
                ((TextBox)o).Text = string.Empty;
            //else if (string.IsNullOrWhiteSpace(txtBox.Text))
            //    txtBox.Text = "0.00";
        };

        protected override void OnAttached()
        {
            AssociatedObject.GotFocus += OnFocusHandler;
            AssociatedObject.LostFocus += OnLostFocusHandler;
        }

        private readonly RoutedEventHandler OnLostFocusHandler = (sender,e) =>
        {
            var txtBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(txtBox.Text))
                txtBox.Text = "0.00";
        };

        protected override void OnDetaching()
        {
            AssociatedObject.GotFocus -= OnFocusHandler;
            AssociatedObject.LostFocus -= OnLostFocusHandler;
        }
    }
}
