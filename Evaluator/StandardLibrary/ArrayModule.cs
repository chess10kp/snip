using Snip.AST;

namespace Snip.Evaluator;

public static class ArrayModule
{
    public static Module CreateModule()
    {
        var module = new Module("array");

        // Creation
        module.Export("create", Value.NativeFunction(new NativeFunctionValue("create", Create)));
        module.Export("range", Value.NativeFunction(new NativeFunctionValue("range", Range)));
        module.Export("fill", Value.NativeFunction(new NativeFunctionValue("fill", Fill)));

        // Information
        module.Export("len", Value.NativeFunction(new NativeFunctionValue("len", Length)));
        module.Export("empty", Value.NativeFunction(new NativeFunctionValue("empty", IsEmpty)));

        // Modification
        module.Export("append", Value.NativeFunction(new NativeFunctionValue("append", Append)));
        module.Export("extend", Value.NativeFunction(new NativeFunctionValue("extend", Extend)));
        module.Export("insert", Value.NativeFunction(new NativeFunctionValue("insert", Insert)));
        module.Export("remove", Value.NativeFunction(new NativeFunctionValue("remove", Remove)));
        module.Export("pop", Value.NativeFunction(new NativeFunctionValue("pop", Pop)));
        module.Export("clear", Value.NativeFunction(new NativeFunctionValue("clear", Clear)));

        // Searching
        module.Export("index", Value.NativeFunction(new NativeFunctionValue("index", Index)));
        module.Export("contains", Value.NativeFunction(new NativeFunctionValue("contains", Contains)));
        module.Export("count", Value.NativeFunction(new NativeFunctionValue("count", Count)));

        // Manipulation
        module.Export("reverse", Value.NativeFunction(new NativeFunctionValue("reverse", Reverse)));
        module.Export("sort", Value.NativeFunction(new NativeFunctionValue("sort", Sort)));
        module.Export("slice", Value.NativeFunction(new NativeFunctionValue("slice", Slice)));
        module.Export("concat", Value.NativeFunction(new NativeFunctionValue("concat", Concat)));

        return module;
    }
    
    private static Value Create(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "create");
        var size = ValidationHelpers.GetInt(args, 0);
        var value = args.Count > 1 ? args[1] : Value.Null();
        var array = new List<Value>();
        for (int i = 0; i < size; i++)
            array.Add(value);
        return Value.Array(array);
    }
    
    private static Value Range(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 3, "range");
        var start = ValidationHelpers.GetInt(args, 0);
        var stop = args.Count > 1 ? ValidationHelpers.GetInt(args, 1) : start;
        var step = args.Count > 2 ? ValidationHelpers.GetInt(args, 2) : 1;
        
        if (args.Count == 1)
        {
            start = 0;
            stop = start;
        }
        
        var array = new List<Value>();
        for (int i = start; i < stop; i += step)
            array.Add(Value.Number(i));
        return Value.Array(array);
    }
    
    private static Value Fill(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "fill");
        var array = ValidationHelpers.GetArray(args, 0);
        var value = args[1];
        for (int i = 0; i < array.Count; i++)
            array[i] = value;
        return Value.Array(array);
    }
    
    private static Value Length(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "len");
        var array = ValidationHelpers.GetArray(args, 0);
        return Value.Number(array.Count);
    }

    private static Value IsEmpty(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "empty");
        var array = ValidationHelpers.GetArray(args, 0);
        return Value.Boolean(array.Count == 0);
    }
    
    private static Value Append(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "append");
        var array = ValidationHelpers.GetArray(args, 0);
        var value = args[1];
        array.Add(value);
        return Value.Array(array);
    }
    
    private static Value Extend(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "extend");
        var array = ValidationHelpers.GetArray(args, 0);
        var other = ValidationHelpers.GetArray(args, 1);
        array.AddRange(other);
        return Value.Array(array);
    }
    
    private static Value Insert(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 3, 3, "insert");
        var array = ValidationHelpers.GetArray(args, 0);
        var index = ValidationHelpers.GetInt(args, 1);
        var value = args[2];
        array.Insert(index, value);
        return Value.Array(array);
    }
    
    private static Value Remove(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "remove");
        var array = ValidationHelpers.GetArray(args, 0);
        var value = args[1];
        array.Remove(value);
        return Value.Array(array);
    }
    
    private static Value Pop(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "pop");
        var array = ValidationHelpers.GetArray(args, 0);
        var index = args.Count > 1 ? ValidationHelpers.GetInt(args, 1) : array.Count - 1;
        var value = array[index];
        array.RemoveAt(index);
        return value;
    }
    
    private static Value Clear(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "clear");
        var array = ValidationHelpers.GetArray(args, 0);
        array.Clear();
        return Value.Array(array);
    }
    
    private static Value Index(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "index");
        var array = ValidationHelpers.GetArray(args, 0);
        var value = args[1];
        var index = array.IndexOf(value);
        return Value.Number(index);
    }
    
    private static Value Contains(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "contains");
        var array = ValidationHelpers.GetArray(args, 0);
        var value = args[1];
        return Value.Boolean(array.Contains(value));
    }
    
    private static Value Count(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "count");
        var array = ValidationHelpers.GetArray(args, 0);
        var value = args[1];
        var count = array.Count(v => v.Equals(value));
        return Value.Number(count);
    }
    
    private static Value Reverse(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "reverse");
        var array = ValidationHelpers.GetArray(args, 0);
        array.Reverse();
        return Value.Array(array);
    }
    
    private static Value Sort(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "sort");
        var array = ValidationHelpers.GetArray(args, 0);
        array.Sort((a, b) => a.ToString().CompareTo(b.ToString()));
        return Value.Array(array);
    }
    
    private static Value Slice(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 4, "slice");
        var array = ValidationHelpers.GetArray(args, 0);
        var start = ValidationHelpers.GetInt(args, 1);
        var end = args.Count > 2 ? ValidationHelpers.GetInt(args, 2) : array.Count;
        var step = args.Count > 3 ? ValidationHelpers.GetInt(args, 3) : 1;
        
        var result = new List<Value>();
        for (int i = start; i < end; i += step)
        {
            if (i >= 0 && i < array.Count)
                result.Add(array[i]);
        }
        return Value.Array(result);
    }
    
    private static Value Concat(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, -1, "concat");
        var result = new List<Value>();
        foreach (var arg in args)
        {
            var array = ValidationHelpers.GetArray(new List<Value> { arg }, 0);
            result.AddRange(array);
        }
        return Value.Array(result);
    }
}