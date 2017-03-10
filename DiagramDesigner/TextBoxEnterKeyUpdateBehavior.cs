using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Security.AccessControl;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace DiagramDesigner
{
    class TextBoxEnterKeyUpdateBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            if(this.AssociatedObject != null)
            {
                base.OnAttached();
                this.AssociatedObject.KeyDown += AssociatedObject_KeyDown;
            }            
        }

        protected override void OnDetaching()
        {
            if(this.AssociatedObject != null)
            {
                this.AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
                base.OnDetaching();
            }            
        }

        private void AssociatedObject_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            MyTextBox textBox = sender as MyTextBox;
            if(textBox != null)
            {
                if(e.Key == Key.Return || e.Key == Key.Enter)
                {
                    textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
        }
    }
}
