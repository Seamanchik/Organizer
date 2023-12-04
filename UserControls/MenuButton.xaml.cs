using System.Windows;
using System.Windows.Controls;

namespace Organizer.UserControls;

public partial class MenuButton : UserControl
{
    public MenuButton()
    {   
        InitializeComponent();
    }

    public string Caption
    {
        get => (string)GetValue(_captionProperty);
        set => SetValue(_captionProperty, value);
    }

    public static readonly DependencyProperty _captionProperty =
        DependencyProperty.Register(nameof(Caption), typeof(string), typeof(MenuButton));
    
    public FontAwesome.WPF.FontAwesomeIcon Icon
    {
        get => (FontAwesome.WPF.FontAwesomeIcon)GetValue(_iconProperty);
        set => SetValue(_iconProperty, value);
    }

    public static readonly DependencyProperty _iconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(FontAwesome.WPF.FontAwesomeIcon), typeof(MenuButton));
}