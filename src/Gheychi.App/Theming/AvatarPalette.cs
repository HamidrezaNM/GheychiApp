using Avalonia.Media;
using Gheychi.Core.Services;

namespace Gheychi.App.Theming;

/// <summary>
/// The eight avatar colours, and the rule that picks one: a hash of the number, so the same contact keeps the same
/// colour in the inbox, the archive, search and the compose list. The colours are the same in both themes, so a number
/// is recognisable at a glance; the ink on each is picked to read against it.
/// </summary>
public static class AvatarPalette
{
    // The tint used before avatars had colours, kept for a row with no number to hash (a sender name).
    private static readonly IBrush FallbackBackground = Palette.Accent(AccentRole.Soft);
    private static readonly IBrush FallbackText = Palette.Accent(AccentRole.Text);

    private static readonly string[] Swatches =
    [
        "#1A73E8", // blue
        "#4ECDE6", // cyan
        "#5BB974", // green
        "#FA903E", // orange
        "#FF63B8", // pink
        "#AF5CF7", // purple
        "#EE675C", // red
        "#FCC934" // yellow
    ];

    // Near black rather than the pure black that would look harsh on the lighter swatches.
    private const string DarkInk = "#1F1F1F";

    private static readonly IBrush[] Backgrounds = [.. Swatches.Select(Palette.Brush)];
    private static readonly IBrush[] Inks = [.. Swatches.Select(ReadableInk)];

    /// <summary>The avatar's fill and the colour of its initial.</summary>
    public static (IBrush Background, IBrush Text) Resolve(string? address)
    {
        var index = IndexOf(address);
        return index < 0 ? (FallbackBackground, FallbackText) : (Backgrounds[index], Inks[index]);
    }

    public static IBrush Background(string? address) => Resolve(address).Background;

    public static IBrush Text(string? address) => Resolve(address).Text;

    /// <summary>
    /// Which swatch an address gets: a hash of its canonical digits, so the colour does not change with how the number
    /// is written (+98 912… and 0912… match). Returns -1 for an empty address.
    /// </summary>
    public static int IndexOf(string? address)
    {
        var key = PhoneNumberNormalizer.ToLookupKey(address ?? string.Empty);
        if (key.Length == 0)
            return -1;

        // FNV-1a: unlike string.GetHashCode, which is salted per process, this keeps the same colour across launches.
        var hash = 2166136261u;
        foreach (var c in key)
        {
            hash = (hash ^ c) * 16777619u;
        }

        return (int)(hash % (uint)Backgrounds.Length);
    }

    // White on the dark swatches, near black on the light ones, so the initial reads on every colour. The 3:1 is the
    // large-text contrast: an avatar's initial is at least 12px bold.
    private static IBrush ReadableInk(string swatch)
    {
        var rgb = Convert.ToInt32(swatch[1..], 16);

        static double Channel(int c)
        {
            var v = c / 255.0;
            return v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
        }

        var luminance = 0.2126 * Channel(rgb >> 16 & 0xFF) + 0.7152 * Channel(rgb >> 8 & 0xFF) +
                        0.0722 * Channel(rgb & 0xFF);
        return 1.05 / (luminance + 0.05) >= 3 ? Palette.Brush("#FFFFFF") : Palette.Brush(DarkInk);
    }
}
