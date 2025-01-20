using System.Reflection;
using Netcode;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffEffects;

public class ProtectionBuffEffects : BuffEffects
{
    public readonly NetFloat ProtectionChance = new NetFloat(0.0f);

    public ProtectionBuffEffects() : base()
    {
        FieldInfo fieldInfo = typeof(BuffEffects).GetField("AdditiveFields", BindingFlags.Instance | BindingFlags.NonPublic);
        NetFloat[] additiveFields = (NetFloat[])fieldInfo.GetValue(this);
        NetFloat[] newAdditiveFields = new NetFloat[additiveFields.Length + 1];
        additiveFields.CopyTo(newAdditiveFields, 0);
        newAdditiveFields[additiveFields.Length] = this.ProtectionChance;
        fieldInfo.SetValue(this, newAdditiveFields);
        
        this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.ProtectionChance, nameof (ProtectionChance));
    }
}