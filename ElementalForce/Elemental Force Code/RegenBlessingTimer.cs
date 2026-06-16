using System.Timers;
using ElementalForce.Elemental_Force_Code.helpers;
using StardewValley;
using Timer = System.Timers.Timer;

namespace ElementalForce.Elemental_Force_Code;

public class RegenBlessingTimer
{
    private Timer timer;

    public RegenBlessingTimer()
    {
        timer = new Timer(BuffConstants.RegenBlessingTickMs);
        timer.Elapsed += TimerElapsed;
        timer.AutoReset = true;
        timer.Start();
    }

    public void UpdateInterval(double interval)
    {
        timer.Interval = interval;
    }

    private void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (Game1.player.health == Game1.player.maxHealth && Game1.player.stamina >= Game1.player.MaxStamina)
        {
            return;
        }

        Game1.player.health += (int) (Game1.player.maxHealth * BuffConstants.RegenBlessingRate);
        if (Game1.player.health > Game1.player.maxHealth)
        {
            Game1.player.health = Game1.player.maxHealth;
        }
        Game1.player.stamina += (float) (Game1.player.MaxStamina * BuffConstants.RegenBlessingRate);
        if (Game1.player.stamina > Game1.player.MaxStamina)
        {
            Game1.player.stamina = Game1.player.MaxStamina;
        }
    }
}
