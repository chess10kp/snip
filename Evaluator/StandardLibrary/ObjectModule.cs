using Snip.AST;

namespace Snip.Evaluator;

public static class ObjectModule
{
    public static Module CreateModule()
    {
        var module = new Module("object");
        
        // Creation
        module.Export("create", Value.NativeFunction(new NativeFunctionValue("create", Create)));
        module.Export("keys", Value.NativeFunction(new NativeFunctionValue("keys", Keys)));
        module.Export("values", Value.NativeFunction(new NativeFunctionValue("values", Values)));
        module.Export("items", Value.NativeFunction(new NativeFunctionValue("items", Items)));
        
        // Information
        module.Export("has", Value.NativeFunction(new NativeFunctionValue("has", Has)));
        module.Export("get", Value.NativeFunction(new NativeFunctionValue("get", Get)));
        module.Export("set", Value.NativeFunction(new NativeFunctionValue("set", Set)));
        module.Export("delete", Value.NativeFunction(new NativeFunctionValue("delete", Delete)));
        module.Export("clear", Value.NativeFunction(new NativeFunctionValue("clear", Clear)));
        
        // Testing
        module.Export("isempty", Value.NativeFunction(new NativeFunctionValue("isempty", IsEmpty)));
        module.Export("size", Value.NativeFunction(new NativeFunctionValue("size", Size)));
        
        // Merging
        module.Export("merge", Value.NativeFunction(new NativeFunctionValue("merge", Merge)));
        module.Export("assign", Value.NativeFunction(new NativeFunctionValue("assign", Assign)));
        
        return module;
    }
    
    private static Value Create(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 0, 1, "create");
        var obj = new Dictionary<string, Value>();
        if (args.Count > 0)
        {
            var source = ValidationHelpers.GetObject(args, 0);
            foreach (var kvp in source)
                obj[kvp.Key] = kvp.Value;
        }
        return Value.Object(obj);
    }
    
    private static Value Keys(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "keys");
        var obj = ValidationHelpers.GetObject(args, 0);
        var keys = obj.Keys.Select(k => Value.String(k)).ToList();
        return Value.Array(keys);
    }
    
    private static Value Values(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "values");
        var obj = ValidationHelpers.GetObject(args, 0);
        return Value.Array(obj.Values.ToList());
    }
    
    private static Value Items(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "items");
        var obj = ValidationHelpers.GetObject(args, 0);
        var items = obj.Select(kvp => Value.Array(new List<Value> { Value.String(kvp.Key), kvp.Value })).ToList();
        return Value.Array(items);
    }
    
    private static Value Has(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "has");
        var obj = ValidationHelpers.GetObject(args, 0);
        var key = ValidationHelpers.GetString(args, 1);
        return Value.Boolean(obj.ContainsKey(key));
    }
    
    private static Value Get(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "get");
        var obj = ValidationHelpers.GetObject(args, 0);
        var key = ValidationHelpers.GetString(args, 1);
        var defaultValue = args.Count > 2 ? args[2] : Value.Null();
        
        return obj.TryGetValue(key, out var value) ? value : defaultValue;
    }
    
    private static Value Set(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 3, 3, "set");
        var obj = ValidationHelpers.GetObject(args, 0);
        var key = ValidationHelpers.GetString(args, 1);
        var value = args[2];
        obj[key] = value;
        return Value.Object(obj);
    }
    
    private static Value Delete(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "delete");
        var obj = ValidationHelpers.GetObject(args, 0);
        var key = ValidationHelpers.GetString(args, 1);
        var removed = obj.Remove(key);
        return Value.Boolean(removed);
    }
    
    private static Value Clear(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "clear");
        var obj = ValidationHelpers.GetObject(args, 0);
        obj.Clear();
        return Value.Object(obj);
    }
    
    private static Value IsEmpty(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "isempty");
        var obj = ValidationHelpers.GetObject(args, 0);
        return Value.Boolean(obj.Count == 0);
    }
    
    private static Value Size(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "size");
        var obj = ValidationHelpers.GetObject(args, 0);
        return Value.Number(obj.Count);
    }
    
    private static Value Merge(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, -1, "merge");
        var result = new Dictionary<string, Value>();
        
        foreach (var arg in args)
        {
            var obj = ValidationHelpers.GetObject(new List<Value> { arg }, 0);
            foreach (var kvp in obj)
                result[kvp.Key] = kvp.Value;
        }
        
        return Value.Object(result);
    }
    
    private static Value Assign(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, -1, "assign");
        var target = ValidationHelpers.GetObject(args, 0);
        
        for (int i = 1; i < args.Count; i++)
        {
            var source = ValidationHelpers.GetObject(new List<Value> { args[i] }, 0);
            foreach (var kvp in source)
                target[kvp.Key] = kvp.Value;
        }
        
        return Value.Object(target);
    }
}