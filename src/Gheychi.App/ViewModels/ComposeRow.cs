using Avalonia.Media;
using Gheychi.App.Theming;

namespace Gheychi.App.ViewModels;

public enum ComposeRowKind
{
    /// <summary>A section title: a letter, or "Recent".</summary>
    Header,
    Contact,
    /// <summary>"Send to 0903…": the number typed in the To field.</summary>
    Typed
}

/// <summary>
/// One row of the new-message list. Rows are built once per contact load and reused by every
/// filter and re-open, so they hold text only and take their colours from the theme, apart from the avatar,
/// which is coloured by the address.
/// </summary>
public sealed class ComposeRow
{
    public required ComposeRowKind Kind { get; init; }

    /// <summary>Title text of a header row.</summary>
    public string Letter { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Subtitle { get; init; } = string.Empty;

    /// <summary>Avatar text; empty when the contact has no usable letters (an icon is shown instead).</summary>
    public string Initials { get; init; } = string.Empty;

    /// <summary>Number to send to (contacts and typed rows).</summary>
    public string Address { get; init; } = string.Empty;

    /// <summary>Contact name for contact rows, null for a typed number.</summary>
    public string? ContactName { get; init; }

    public string? PhotoAddress => ContactName is null ? null : Address;

    public bool HasSubtitle => Subtitle.Length > 0;
    public bool HasInitials => Initials.Length > 0;
    public bool HasNoInitials => Initials.Length == 0;

    public IBrush AvatarBgColor => AvatarColors.Background;
    public IBrush AvatarTextColor => AvatarColors.Text;

    // Worked out once, when the row is first drawn.
    private (IBrush Background, IBrush Text)? _avatarColors;
    private (IBrush Background, IBrush Text) AvatarColors => _avatarColors ??= AvatarPalette.Resolve(Address);
}

public sealed record ComposeRecipient(string Address, string? ContactName, Gheychi.Core.Models.SimCardInfo? Sim);
