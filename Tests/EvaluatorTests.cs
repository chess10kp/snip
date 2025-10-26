using Xunit;
using Snip.AST;
using Snip.Lexer;
using Snip.Parser;
using Snip.Evaluator;

namespace Tests;

public class EvaluatorTests
{
    private Value Evaluate(string input)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.Parse();
        var evaluator = new Evaluator();
        var env = new Snip.Evaluator.Environment();
        return evaluator.Eval(program, env);
    }

    [Fact]
    public void EvalIntegerLiteral_ShouldReturnIntegerValue()
    {
        var result = Evaluate("42;");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(42L, result.Data);
    }

    [Fact]
    public void EvalFunctionDeclaration_ShouldDefineFunction()
    {
        var result = Evaluate("function add(a, b) { return a + b; } add;");
        Assert.Equal(Snip.Evaluator.ValueType.Function, result.Type);
    }

    [Fact]
    public void EvalFunctionCall_ShouldExecuteFunction()
    {
        var result = Evaluate("function add(a, b) { return a + b; } add(2, 3);");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type);
        Assert.Equal(5.0, result.Data);
    }

    [Fact]
    public void EvalFunctionCallWithReturn_ShouldReturnValue()
    {
        var result = Evaluate("function test() { return 42; } test();");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(42L, result.Data);
    }

    [Fact]
    public void EvalObjectLiteral_ShouldCreateObject()
    {
        var result = Evaluate("{ \"name\": \"John\", \"age\": 30 };");
        Assert.Equal(Snip.Evaluator.ValueType.Object, result.Type);
        var obj = (Dictionary<string, Value>)result.Data!;
        Assert.Equal("John", obj["name"].Data);
        Assert.Equal(30L, obj["age"].Data);
    }

    [Fact]
    public void EvalMemberAccess_ShouldReturnPropertyValue()
    {
        var result = Evaluate("let obj = { \"name\": \"John\" }; obj.name;");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("John", result.Data);
    }

    [Fact]
    public void EvalArrayLiteral_ShouldCreateArray()
    {
        var result = Evaluate("[1, 2, 3];");
        Assert.Equal(Snip.Evaluator.ValueType.Array, result.Type);
        var arr = (List<Value>)result.Data!;
        Assert.Equal(3, arr.Count);
        Assert.Equal(1L, arr[0].Data);
        Assert.Equal(2L, arr[1].Data);
        Assert.Equal(3L, arr[2].Data);
    }

    [Fact]
    public void EvalCompoundAssignment_ShouldWorkCorrectly()
    {
        var result = Evaluate("let x = 5; x += 3; x;");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type);
        Assert.Equal(8.0, result.Data);

        result = Evaluate("let y = 10; y *= 2; y;");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type);
        Assert.Equal(20.0, result.Data);
    }

    [Fact]
    public void EvalVarStatement_ShouldDefineVariable()
    {
        var result = Evaluate("var x = 42; x;");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(42L, result.Data);
    }

    [Fact]
    public void EvalConstStatement_ShouldDefineConstant()
    {
        var result = Evaluate("const PI = 3.14; PI;");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type);
        Assert.Equal(3.14, result.Data);
    }

    [Fact]
    public void EvalStringLiteral_ShouldReturnStringValue()
    {
        var result = Evaluate("\"hello world\";");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("hello world", result.Data);
    }

    [Fact]
    public void EvalBooleanLiteral_ShouldReturnBooleanValue()
    {
        var result = Evaluate("true;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.True((bool)result.Data!);

        result = Evaluate("false;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.False((bool)result.Data!);
    }

    [Fact]
    public void EvalNullLiteral_ShouldReturnNullValue()
    {
        var result = Evaluate("null;");
        Assert.Equal(Snip.Evaluator.ValueType.Null, result.Type);
    }

    [Fact]
    public void EvalUndefinedLiteral_ShouldReturnUndefinedValue()
    {
        var result = Evaluate("undefined;");
        Assert.Equal(Snip.Evaluator.ValueType.Undefined, result.Type);
    }

    [Fact]
    public void EvalAddExpression_ShouldReturnSum()
    {
        var result = Evaluate("1 + 2;");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type);
        Assert.Equal(3.0, result.Data);
    }

    [Fact]
    public void EvalSubtractExpression_ShouldReturnDifference()
    {
        var result = Evaluate("5 - 3;");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type);
        Assert.Equal(2.0, result.Data);
    }

    [Fact]
    public void EvalMultiplyExpression_ShouldReturnProduct()
    {
        var result = Evaluate("4 * 3;");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type);
        Assert.Equal(12.0, result.Data);
    }

    [Fact]
    public void EvalDivideExpression_ShouldReturnQuotient()
    {
        var result = Evaluate("8 / 2;");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type);
        Assert.Equal(4.0, result.Data);
    }

    [Fact]
    public void EvalModuloExpression_ShouldReturnRemainder()
    {
        var result = Evaluate("7 % 3;");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type);
        Assert.Equal(1.0, result.Data);
    }

    [Fact]
    public void EvalEqualExpression_ShouldReturnBoolean()
    {
        var result = Evaluate("1 == 1;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.True((bool)result.Data!);

        result = Evaluate("1 == 2;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.False((bool)result.Data!);
    }

    [Fact]
    public void EvalNotEqualExpression_ShouldReturnBoolean()
    {
        var result = Evaluate("1 != 2;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.True((bool)result.Data!);

        result = Evaluate("1 != 1;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.False((bool)result.Data!);
    }

    [Fact]
    public void EvalLessThanExpression_ShouldReturnBoolean()
    {
        var result = Evaluate("1 < 2;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.True((bool)result.Data!);

        result = Evaluate("2 < 1;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.False((bool)result.Data!);
    }

    [Fact]
    public void EvalGreaterThanExpression_ShouldReturnBoolean()
    {
        var result = Evaluate("2 > 1;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.True((bool)result.Data!);

        result = Evaluate("1 > 2;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.False((bool)result.Data!);
    }

    [Fact]
    public void EvalStringConcatenation_ShouldReturnConcatenatedString()
    {
        var result = Evaluate("\"hello\" + \" \" + \"world\";");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("hello world", result.Data);
    }

    [Fact]
    public void EvalLetStatement_ShouldDefineVariable()
    {
        var result = Evaluate("let x = 42; x;");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(42L, result.Data);
    }

    [Fact]
    public void EvalAssignmentExpression_ShouldUpdateVariable()
    {
        var result = Evaluate("let x = 1; x = 2; x;");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(2L, result.Data);
    }

    [Fact]
    public void EvalIfStatement_TrueCondition_ShouldExecuteConsequence()
    {
        var result = Evaluate("if (true) { 42; }");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(42L, result.Data);
    }

    [Fact]
    public void EvalIfStatement_FalseCondition_ShouldReturnUndefined()
    {
        var result = Evaluate("if (false) { 42; }");
        Assert.Equal(Snip.Evaluator.ValueType.Undefined, result.Type);
    }

    [Fact]
    public void EvalIfElseStatement_TrueCondition_ShouldExecuteConsequence()
    {
        var result = Evaluate("if (true) { 42; } else { 24; }");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(42L, result.Data);
    }

    [Fact]
    public void EvalIfElseStatement_FalseCondition_ShouldExecuteAlternative()
    {
        var result = Evaluate("if (false) { 42; } else { 24; }");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(24L, result.Data);
    }

    [Fact]
    public void EvalConditionalExpression_TrueCondition_ShouldReturnConsequent()
    {
        var result = Evaluate("true ? 42 : 24;");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(42L, result.Data);
    }

    [Fact]
    public void EvalConditionalExpression_FalseCondition_ShouldReturnAlternative()
    {
        var result = Evaluate("false ? 42 : 24;");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(24L, result.Data);
    }

    [Fact]
    public void EvalBlockStatement_ShouldReturnLastExpression()
    {
        var result = Evaluate("{ let x = 1; let y = 2; x + y; }");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type);
        Assert.Equal(3.0, result.Data);
    }

    [Fact]
    public void EvalBlockStatement_Scoping_ShouldIsolateVariables()
    {
        var result = Evaluate("let x = 1; { let x = 2; x; } x;");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(1L, result.Data);
    }

    [Fact]
    public void EvalComplexExpression_ShouldWorkCorrectly()
    {
        var result = Evaluate("let x = 10; if (x > 5) { x * 2; } else { x / 2; }");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type);
        Assert.Equal(20.0, result.Data);
    }

    [Fact]
    public void EvalWhileLoop_ShouldExecuteUntilConditionFalse()
    {
        var result = Evaluate("let x = 0; while (x < 3) { x = x + 1; } x;");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type); // Addition returns float
        Assert.Equal(3.0, result.Data);
    }

    [Fact]
    public void EvalWhileLoop_ZeroIterations_ShouldNotExecuteBody()
    {
        var result = Evaluate("let x = 5; while (x < 3) { x = x + 1; } x;");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(5L, result.Data);
    }

    [Fact]
    public void EvalForLoop_ShouldExecuteCorrectly()
    {
        var result = Evaluate("let sum = 0; for (let i = 0; i < 5; i = i + 1) { sum = sum + i; } sum;");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type); // Addition returns float
        Assert.Equal(10.0, result.Data); // 0+1+2+3+4 = 10
    }

    [Fact]
    public void EvalForLoop_WithoutInitializer_ShouldWork()
    {
        var result = Evaluate("let i = 0; let sum = 0; for (; i < 3; i = i + 1) { sum = sum + i; } sum;");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type); // Addition returns float
        Assert.Equal(3.0, result.Data); // 0+1+2 = 3
    }

    [Fact]
    public void EvalUnaryNotOperator_ShouldNegateBoolean()
    {
        var result = Evaluate("!true;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.False((bool)result.Data!);

        result = Evaluate("!false;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.True((bool)result.Data!);
    }

    [Fact]
    public void EvalUnaryMinusOperator_ShouldNegateNumber()
    {
        var result = Evaluate("-42;");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(-42L, result.Data);

        result = Evaluate("-3.14;");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type);
        Assert.Equal(-3.14, result.Data);
    }

    [Fact]
    public void EvalUnaryPlusOperator_ShouldReturnNumber()
    {
        var result = Evaluate("+42;");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(42L, result.Data);

        result = Evaluate("+3.14;");
        Assert.Equal(Snip.Evaluator.ValueType.Float, result.Type);
        Assert.Equal(3.14, result.Data);
    }

    [Fact]
    public void EvalClassDeclaration_ShouldDefineClass()
    {
        var result = Evaluate("class Test {} Test;");
        Assert.Equal(Snip.Evaluator.ValueType.Class, result.Type);
    }

    [Fact]
    public void EvalNewExpression_ShouldCreateInstance()
    {
        var result = Evaluate("class Test {} new Test();");
        Assert.Equal(Snip.Evaluator.ValueType.Instance, result.Type);
    }

    [Fact]
    public void EvalClassWithProperty_ShouldInitializeProperty()
    {
        var result = Evaluate("class Test { x = 42; } let t = new Test(); t.x;");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(42L, result.Data);
    }

    [Fact]
    public void EvalClassWithMethod_ShouldExecuteMethod()
    {
        var result = Evaluate("class Test { getValue() { return 42; } } let t = new Test(); t.getValue();");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(42L, result.Data);
    }

    [Fact]
    public void EvalClassWithConstructor_ShouldExecuteConstructor()
    {
        var result = Evaluate("class Test { constructor() { this.value = 42; } } let t = new Test(); t.value;");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(42L, result.Data);
    }

    [Fact]
    public void EvalThisInMethod_ShouldReferToInstance()
    {
        var result = Evaluate("class Test { setValue(v) { this.value = v; } } let t = new Test(); t.setValue(123); t.value;");
        Assert.Equal(Snip.Evaluator.ValueType.Integer, result.Type);
        Assert.Equal(123L, result.Data);
    }

    [Fact]
    public void EvalClassInheritance_ShouldInheritProperties()
    {
        var result = Evaluate("class Animal { name = \"animal\"; } class Dog extends Animal {} let d = new Dog(); d.name;");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("animal", result.Data);
    }

    [Fact]
    public void EvalClassInheritance_ShouldInheritMethods()
    {
        var result = Evaluate("class Animal { speak() { return \"sound\"; } } class Dog extends Animal {} let d = new Dog(); d.speak();");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("sound", result.Data);
    }

    [Fact]
    public void EvalSuperConstructor_ShouldCallParentConstructor()
    {
        var result = Evaluate("class Animal { constructor(name) { this.name = name; } } class Dog extends Animal { constructor(name, breed) { super(name); this.breed = breed; } } let d = new Dog(\"Rex\", \"Labrador\"); d.name;");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("Rex", result.Data);
    }

    [Fact]
    public void EvalSuperMethod_ShouldCallParentMethod()
    {
        var result = Evaluate("class Animal { speak() { return \"animal sound\"; } } class Dog extends Animal { speak() { return super.speak() + \" woof\"; } } let d = new Dog(); d.speak();");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("animal sound woof", result.Data);
    }

    [Fact]
    public void EvalSuperProperty_ShouldAccessParentProperty()
    {
        var result = Evaluate("class Animal { species = \"animal\"; } class Dog extends Animal { getSpecies() { return super.species; } } let d = new Dog(); d.getSpecies();");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("animal", result.Data);
    }
}