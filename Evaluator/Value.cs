namespace Snip.Evaluator;

public enum ValueType
{
    Integer,
    Float,
    String,
    Boolean,
    Null,
    Undefined,
    Array,
    Object,
    Function
}

public class Value
{
    public ValueType Type { get; }
    public object? Data { get; }

    public Value(ValueType type, object? data = null)
    {
        Type = type;
        Data = data;
    }

    // Convenience constructors
    public static Value Integer(long value) => new(ValueType.Integer, value);
    public static Value Float(double value) => new(ValueType.Float, value);
    public static Value String(string value) => new(ValueType.String, value);
    public static Value Boolean(bool value) => new(ValueType.Boolean, value);
    public static Value Null() => new(ValueType.Null);
    public static Value Undefined() => new(ValueType.Undefined);
    public static Value Array(List<Value> values) => new(ValueType.Array, values);
    public static Value Object(Dictionary<string, Value> properties) => new(ValueType.Object, properties);

    public override string ToString()
    {
        return Type switch
        {
            ValueType.Integer => Data?.ToString() ?? "0",
            ValueType.Float => Data?.ToString() ?? "0.0",
            ValueType.String => $"\"{Data}\"",
            ValueType.Boolean => Data?.ToString() ?? "false",
            ValueType.Null => "null",
            ValueType.Undefined => "undefined",
            ValueType.Array => $"[{string.Join(", ", ((List<Value>)Data!).Select(v => v.ToString()))}]",
            ValueType.Object => $"{{{string.Join(", ", ((Dictionary<string, Value>)Data!).Select(kv => $"\"{kv.Key}\": {kv.Value}"))}}}",
            ValueType.Function => "[Function]",
            _ => "unknown"
        };
    }
}