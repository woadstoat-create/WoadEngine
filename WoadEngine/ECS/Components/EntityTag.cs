namespace WoadEngine.ECS.Components;

public struct EntityTag
{
    public string Tag;

    public static EntityTag Create(string tag)
    {
        return new EntityTag
        {
            Tag = tag
        };
    }
}