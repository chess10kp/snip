namespace Snip.Evaluator;

public class Environment
{
    private readonly Dictionary<string, Value> _bindings = new();
    private readonly Environment? _parent;

    public Environment(Environment? parent = null)
    {
        _parent = parent;
    }

    public void Define(string name, Value value)
    {
        _bindings[name] = value;
    }

    public Value Get(string name)
    {
        if (_bindings.TryGetValue(name, out var value))
        {
            return value;
        }

        if (_parent != null)
        {
            return _parent.Get(name);
        }

        throw new Exception($"Undefined variable: {name}");
    }

    public void Assign(string name, Value value)
    {
        if (_bindings.ContainsKey(name))
        {
            _bindings[name] = value;
            return;
        }

        if (_parent != null)
        {
            _parent.Assign(name, value);
            return;
        }

        // Allow assignment to create variables (like JavaScript)
        _bindings[name] = value;
    }

    public Environment CreateChild()
    {
        return new Environment(this);
    }
}