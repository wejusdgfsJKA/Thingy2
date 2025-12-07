using System.Collections.Generic;

public class Context<T>
{
    public readonly Ship Ship;
    public readonly Navigation Navigation;
    protected Dictionary<T, object> data = new();
    public Context(Ship ship)
    {
        Ship = ship;
        Navigation = Ship.Navigation;
    }
    public void SetData<R>(T key, R value) => data[key] = value;
    public R GetData<R>(T key)
    {
        return data.TryGetValue(key, out var value) ? (R)value : default;
    }
}
