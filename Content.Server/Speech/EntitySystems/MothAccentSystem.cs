using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Robust.Shared.Random;
using Content.Server._Triad.Speech.EntitySystems; // Triad: AccentHelpers relocated to _Triad

namespace Content.Server.Speech.EntitySystems;

public sealed partial class MothAccentSystem : EntitySystem
{
    [Dependency] private IRobustRandom _random = default!; // Triad: for the flutter tic

    private static readonly Regex RegexLowerBuzz = new Regex("z{1,3}");
    private static readonly Regex RegexUpperBuzz = new Regex("Z{1,3}");
    // Cheeburbr start. Локалізація
    private static readonly Regex RegexLowerCyrillicBzhh = new Regex("ж{1,3}");
    private static readonly Regex RegexUpperCyrillicBzhh = new Regex("Ж{1,3}");
    private static readonly Regex RegexLowerCyrillicBzz = new Regex("з{1,3}");
    private static readonly Regex RegexUpperCyrillicBzz = new Regex("З{1,3}");
    // Cheeburbr end

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MothAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, MothAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // buzzz
        message = RegexLowerBuzz.Replace(message, "zzz");
        // buZZZ
        message = RegexUpperBuzz.Replace(message, "ZZZ");
        // Cheeburbr start. Локалізація
        // бжжж
        message = RegexLowerCyrillicBzhh.Replace(message, "жжж");
        // БЖЖЖ
        message = RegexUpperCyrillicBzhh.Replace(message, "ЖЖЖ");
        // бззз
        message = RegexLowerCyrillicBzz.Replace(message, "ззз");
        // БЗЗЗ
        message = RegexUpperCyrillicBzz.Replace(message, "ЗЗЗ");
        // Cheeburbr end

        // Triad: occasional fluttery wingbeat so moth speech reads as moth even on z-less lines.
        if (component.Flutters.Count > 0 && _random.Prob(component.FlutterChance))
            message = AccentHelpers.AppendSuffix(message, Loc.GetString(_random.Pick(component.Flutters)));

        args.Message = message;
    }
}
