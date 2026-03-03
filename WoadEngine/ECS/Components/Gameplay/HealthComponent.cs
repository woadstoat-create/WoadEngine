namespace WoadEngine.ECS.Components.Gameplay;

public struct HealthComponent
{
    public int MaxHealth;
    public int CurrentHealth;
    public bool IsInvincible;

    public static HealthComponent Create(int max, bool invincible = false)
    {
        return new HealthComponent
        {
            MaxHealth = max,
            CurrentHealth = max,
            IsInvincible = invincible
        };
    }
}