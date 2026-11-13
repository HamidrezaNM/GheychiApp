using Avalonia.Controls;
using Gheychi.App.Theming;
using Gheychi.App.Ui;
using Gheychi.App.ViewModels;

namespace Gheychi.App.Controls;

public partial class ContactChip : UserControl
{
    public ContactChip()
    {
        InitializeComponent();
    }

    /// <summary>The number as it was picked out of the message.</summary>
    public string Number { get; private set; } = string.Empty;

    public void Show(string name, string number)
    {
        Number = number;
        ContactName.Text = name;
        var (background, ink) = AvatarPalette.Resolve(number);
        AvatarCircle.Background = background;
        ContactIcon.Foreground = ink;
        ContactInitials.Foreground = ink;
        var icon = IconCatalog.Find(ThreadItem.DetectIcon(name, number));
        ContactIcon.Data = icon;
        ContactIcon.IsVisible = icon is not null;
        ContactInitials.Text = ThreadItem.GenerateInitials(name);
        ContactInitials.IsVisible = icon is null;
    }
}
