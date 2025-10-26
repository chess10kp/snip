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
    public void EvalFloatLiteral_ShouldReturnFloatValue()
    {
        var result = Evaluate("3.14;");
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
}