using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TaskSharp.Themes
{
    public partial class TextboxTheme : ResourceDictionary

    {
        public void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Debug.WriteLine((sender as TextBox).Text);
            }
        }
    }
}
