using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using System.Linq;

namespace AKRound;

[MinimumApiVersion(160)]
public class AKRound : BasePlugin
{
    public override string ModuleName => "AK-47 Round";
    public override string ModuleVersion => "1.1.2";
    public override string ModuleAuthor => "NicoV";

    private bool nextRoundIsAKRound = false;
    private bool isSpecialRoundActive = false;

    public override void Load(bool hotReload)
    {
        AddCommand("css_akround", "Bir sonraki elin AK-47 turu olmasini saglar.", OnAKRoundCommand);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
    }

    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if (nextRoundIsAKRound)
        {
            Server.PrintToChatAll($"{ChatColors.LightRed}[ ÖZEL TUR ]{ChatColors.White} -------------------------------------");
            Server.PrintToChatAll($"{ChatColors.Gold}▶ {ChatColors.Yellow}Bu el AK-47 ve Deagle turudur! {ChatColors.Gold}◀");
            Server.PrintToChatAll($"{ChatColors.LightRed}[ ÖZEL TUR ]{ChatColors.White} -------------------------------------");

            AddTimer(1.0f, () => GiveAK47ToPlayers());

            nextRoundIsAKRound = false;
            isSpecialRoundActive = true;
        }

        return HookResult.Continue;
    }

    private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if (isSpecialRoundActive)
        {
            RemoveAllWeaponsFromPlayers();
            isSpecialRoundActive = false;
            Server.PrintToChatAll($"{ChatColors.LightRed}[ ÖZEL TUR ]{ChatColors.White} Ozel tur sona erdi. Envanterler temizlendi.");
        }

        return HookResult.Continue;
    }

    [RequiresPermissions("@css/root")]
    private void OnAKRoundCommand(CCSPlayerController? caller, CommandInfo info)
    {
        if (caller == null)
        {
            nextRoundIsAKRound = true;
            Server.PrintToChatAll($"{ChatColors.LightRed}[ ÖZEL TUR ]{ChatColors.White} Konsoldan AK Turu emri verildi. Bir sonraki el basliyor...");
            return;
        }

        nextRoundIsAKRound = true;
        caller.PrintToChat($"{ChatColors.Gold}[ AK Turu ]{ChatColors.White} Bir sonraki el AK-47 turu olarak ayarlandi!");
        Server.PrintToChatAll($"{ChatColors.LightRed}[ ÖZEL TUR ]{ChatColors.White} {ChatColors.Green}{caller.PlayerName}{ChatColors.White} bir sonraki elin AK-47 turu olmasina karar verdi!");
    }

    private void GiveAK47ToPlayers()
    {
        foreach (var player in Utilities.GetPlayers().Where(p => p != null && p.IsValid && !p.IsBot && p.PawnIsAlive))
        {
            player.RemoveWeapons();

            player.GiveNamedItem("weapon_knife");
            player.GiveNamedItem("weapon_ak47");
            player.GiveNamedItem("weapon_deagle");
            player.GiveNamedItem("item_assaultsuit");
        }
    }

    private void RemoveAllWeaponsFromPlayers()
    {
        foreach (var player in Utilities.GetPlayers().Where(p => p != null && p.IsValid && !p.IsBot && p.PawnIsAlive))
        {
            player.RemoveWeapons();
        }
    }
}