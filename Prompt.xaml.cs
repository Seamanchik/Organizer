using System.Windows;

namespace Organizer;

public partial class Prompt : Window
{
    public Prompt()
    {
        InitializeComponent();
    }
    
    public string ResponseText {
        get => ResponseTextBox.Text;
        set => ResponseTextBox.Text = value;
    }

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}