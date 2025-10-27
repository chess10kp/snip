using Snip.AST;

namespace Snip.Evaluator;

public class FunctionValue
{
    public List<AstNode> Parameters { get; }
    public BlockStatementNode Body { get; }
    public Environment Closure { get; }

    public FunctionValue(List<AstNode> parameters, BlockStatementNode body, Environment closure)
    {
        Parameters = parameters;
        Body = body;
        Closure = closure;
    }
}

public class ClassValue
{
    public string Name { get; }
    public ClassValue? SuperClass { get; }
    public Dictionary<string, ClassMember> Members { get; }
    public Environment Closure { get; }

    public ClassValue(string name, ClassValue? superClass, Dictionary<string, ClassMember> members, Environment closure)
    {
        Name = name;
        SuperClass = superClass;
        Members = members;
        Closure = closure;
    }
}

public class ClassInstance
{
    public ClassValue Class { get; }
    public Dictionary<string, Value> Properties { get; }

    public ClassInstance(ClassValue @class, Dictionary<string, Value> properties)
    {
        Class = @class;
        Properties = properties;
    }
}

public class ClassMember
{
    public string Name { get; }
    public string Visibility { get; }
    public bool IsStatic { get; }
    public bool IsReadonly { get; }
    public Value? Value { get; }

    public ClassMember(string name, string visibility, bool isStatic, bool isReadonly, Value? value)
    {
        Name = name;
        Visibility = visibility;
        IsStatic = isStatic;
        IsReadonly = isReadonly;
        Value = value;
    }
}

public class ReturnValue
{
    public Value Value { get; }

    public ReturnValue(Value value)
    {
        Value = value;
    }
}

public class BreakValue
{
    public Value Value { get; }

    public BreakValue(Value value)
    {
        Value = value;
    }
}

public class ContinueValue
{
    public Value Value { get; }

    public ContinueValue(Value value)
    {
        Value = value;
    }
}

public class ExceptionValue
{
    public Value Value { get; }

    public ExceptionValue(Value value)
    {
        Value = value;
    }
}

public delegate Value NativeFunctionDelegate(List<Value> args);

public class NativeFunctionValue
{
    public string Name { get; }
    public NativeFunctionDelegate Function { get; }

    public NativeFunctionValue(string name, NativeFunctionDelegate function)
    {
        Name = name;
        Function = function;
    }
}

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
    Function,
    Class,
    Instance,
    Return,
    Break,
    Continue,
    Exception,
    NativeFunction
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
    public static Value Function(FunctionValue function) => new(ValueType.Function, function);
    public static Value Class(ClassValue @class) => new(ValueType.Class, @class);
    public static Value Instance(ClassInstance instance) => new(ValueType.Instance, instance);
    public static Value Return(ReturnValue returnValue) => new(ValueType.Return, returnValue);
    public static Value Break(BreakValue breakValue) => new(ValueType.Break, breakValue);
    public static Value Continue(ContinueValue continueValue) => new(ValueType.Continue, continueValue);
    public static Value Exception(ExceptionValue exceptionValue) => new(ValueType.Exception, exceptionValue);
    public static Value NativeFunction(NativeFunctionValue nativeFunction) => new(ValueType.NativeFunction, nativeFunction);

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
            ValueType.Class => $"[class {(ClassValue)Data!}]",
            ValueType.Instance => $"[object {(ClassInstance)Data!}]",
            ValueType.Break => "[break]",
            ValueType.Continue => "[continue]",
            ValueType.Exception => $"[exception {(ExceptionValue)Data!}]",
            ValueType.NativeFunction => $"[native function {(NativeFunctionValue)Data!}]",
            _ => "unknown"
        };
    }
}