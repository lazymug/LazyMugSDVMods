using ElementalForce.Elemental_Force_Code.helpers;
using StardewValley;

namespace ElementalForce.Elemental_Force_Code;

public class RegenBlessingTimer
{
    private const int TicksPerSecond = 60;

    private int _tickCounter;
    private int _intervalTicks;

    public RegenBlessingTimer()
    {
        _intervalTicks = MsToTicks(BuffConstants.RegenBlessingTickMs);
        _tickCounter = 0;
    }

    public void UpdateInterval(int intervalMs)
    {
        _intervalTicks = MsToTicks(intervalMs);
    }

    public void Tick()
    {
        _tickCounter++;
        if (_tickCounter < _intervalTicks)
            return;

        _tickCounter = 0;

        if (Game1.player.health == Game1.player.maxHealth && Game1.player.stamina >= Game1.player.MaxStamina)
            return;

        Game1.player.health += (int)(Game1.player.maxHealth * ModEntry.Instance.Config.RegenBlessingRate);
        if (Game1.player.health > Game1.player.maxHealth)
            Game1.player.health = Game1.player.maxHealth;

        Game1.player.stamina += (float)(Game1.player.MaxStamina * ModEntry.Instance.Config.RegenBlessingRate);
        if (Game1.player.stamina > Game1.player.MaxStamina)
            Game1.player.stamina = Game1.player.MaxStamina;
    }

    private static int MsToTicks(int ms) => ms * TicksPerSecond / 1000;
}
