
/// <summary>
/// 若一个类实现了该接口，
/// 则ObjectManager.DestroyObject(...)能够析构该类的实例，以及所有实现Desroyable的成员。
/// </summary>
public interface Destroyable
{
    public void Destroy();

    public void DestroyImmediate();
}

public class ObjectManager
{
    public static void DestroyObject(Destroyable de)
    {
        de.Destroy();
        foreach(var field in de.GetType().GetFields())
        {
            if (field.FieldType.IsSubclassOf(typeof(Destroyable)))
            {
                object f_v = field.GetValue(field);
                if(f_v != null)
                {
                    ((Destroyable)f_v).Destroy();
                }
            }
        }
    }
    public static void DestroyImmediate(Destroyable de)
    {
        de.DestroyImmediate();
        foreach (var field in de.GetType().GetFields())
        {
            if (field.FieldType.IsSubclassOf(typeof(Destroyable)))
            {
                object f_v = field.GetValue(field);
                if (f_v != null)
                {
                    ((Destroyable)f_v).DestroyImmediate();
                }
            }
        }
    }
}