using Snip.AST;

namespace Snip.Evaluator;

public class Evaluator
{
    public Value Eval(AstNode node, Environment env)
    {
        return node switch
        {
            ProgramNode program => EvalProgram(program, env),
            IntegerLiteralNode intLit => Value.Integer(intLit.Value),
            FloatLiteralNode floatLit => Value.Float(floatLit.Value),
            StringLiteralNode strLit => Value.String(strLit.Value),
            BooleanLiteralNode boolLit => Value.Boolean(boolLit.Value),
            NullLiteralNode => Value.Null(),
            UndefinedLiteralNode => Value.Undefined(),
            IdentifierNode ident => env.Get(ident.Name),
            BinaryExpressionNode binExpr => EvalBinary(binExpr, env),
            AddExpressionNode addExpr => EvalBinary(addExpr.Left, addExpr.Right, "+", env),
            SubtractExpressionNode subExpr => EvalBinary(subExpr.Left, subExpr.Right, "-", env),
            MultiplyExpressionNode mulExpr => EvalBinary(mulExpr.Left, mulExpr.Right, "*", env),
            DivideExpressionNode divExpr => EvalBinary(divExpr.Left, divExpr.Right, "/", env),
            ModuloExpressionNode modExpr => EvalBinary(modExpr.Left, modExpr.Right, "%", env),
            EqualExpressionNode eqExpr => EvalBinary(eqExpr.Left, eqExpr.Right, "==", env),
            NotEqualExpressionNode neExpr => EvalBinary(neExpr.Left, neExpr.Right, "!=", env),
            LessThanExpressionNode ltExpr => EvalBinary(ltExpr.Left, ltExpr.Right, "<", env),
            LessThanOrEqualExpressionNode leExpr => EvalBinary(leExpr.Left, leExpr.Right, "<=", env),
            GreaterThanExpressionNode gtExpr => EvalBinary(gtExpr.Left, gtExpr.Right, ">", env),
            GreaterThanOrEqualExpressionNode geExpr => EvalBinary(geExpr.Left, geExpr.Right, ">=", env),
            AssignmentExpressionNode assignExpr => EvalAssignment(assignExpr, env),
            LetStatementNode letStmt => EvalLet(letStmt, env),
            ExpressionStatementNode exprStmt => Eval(exprStmt.Expression, env),
            _ => throw new NotImplementedException($"Evaluation not implemented for {node.NodeType}")
        };
    }

    private Value EvalBinary(ExpressionNode leftNode, ExpressionNode rightNode, string op, Environment env)
    {
        var left = Eval(leftNode, env);
        var right = Eval(rightNode, env);

        return op switch
        {
            "+" => EvalAdd(left, right),
            "-" => EvalSubtract(left, right),
            "*" => EvalMultiply(left, right),
            "/" => EvalDivide(left, right),
            "%" => EvalModulo(left, right),
            "==" => Value.Boolean(ValuesEqual(left, right)),
            "!=" => Value.Boolean(!ValuesEqual(left, right)),
            "<" => Value.Boolean(CompareValues(left, right) < 0),
            "<=" => Value.Boolean(CompareValues(left, right) <= 0),
            ">" => Value.Boolean(CompareValues(left, right) > 0),
            ">=" => Value.Boolean(CompareValues(left, right) >= 0),
            "&&" => Value.Boolean(IsTruthy(left) && IsTruthy(right)),
            "||" => Value.Boolean(IsTruthy(left) || IsTruthy(right)),
            _ => throw new NotImplementedException($"Binary operator {op} not implemented")
        };
    }

    private Value EvalBinary(BinaryExpressionNode node, Environment env)
    {
        return EvalBinary(node.Left, node.Right, node.Operator, env);
    }

    private Value EvalAdd(Value left, Value right)
    {
        if (left.Type == ValueType.String || right.Type == ValueType.String)
        {
            return Value.String(left.ToString() + right.ToString());
        }

        var leftNum = ToNumber(left);
        var rightNum = ToNumber(right);
        return Value.Float(leftNum + rightNum);
    }

    private Value EvalSubtract(Value left, Value right)
    {
        var leftNum = ToNumber(left);
        var rightNum = ToNumber(right);
        return Value.Float(leftNum - rightNum);
    }

    private Value EvalMultiply(Value left, Value right)
    {
        var leftNum = ToNumber(left);
        var rightNum = ToNumber(right);
        return Value.Float(leftNum * rightNum);
    }

    private Value EvalDivide(Value left, Value right)
    {
        var leftNum = ToNumber(left);
        var rightNum = ToNumber(right);
        return Value.Float(leftNum / rightNum);
    }

    private Value EvalModulo(Value left, Value right)
    {
        var leftNum = ToNumber(left);
        var rightNum = ToNumber(right);
        return Value.Float(leftNum % rightNum);
    }

    private double ToNumber(Value value)
    {
        return value.Type switch
        {
            ValueType.Integer => (long)value.Data!,
            ValueType.Float => (double)value.Data!,
            _ => throw new InvalidOperationException($"Cannot convert {value.Type} to number")
        };
    }

    private bool ValuesEqual(Value left, Value right)
    {
        if (left.Type != right.Type) return false;

        return left.Type switch
        {
            ValueType.Integer => (long)left.Data! == (long)right.Data!,
            ValueType.Float => (double)left.Data! == (double)right.Data!,
            ValueType.String => (string)left.Data! == (string)right.Data!,
            ValueType.Boolean => (bool)left.Data! == (bool)right.Data!,
            ValueType.Null => true,
            ValueType.Undefined => true,
            _ => false
        };
    }

    private int CompareValues(Value left, Value right)
    {
        var leftNum = ToNumber(left);
        var rightNum = ToNumber(right);
        return leftNum.CompareTo(rightNum);
    }

    private bool IsTruthy(Value value)
    {
        return value.Type switch
        {
            ValueType.Boolean => (bool)value.Data!,
            ValueType.Null => false,
            ValueType.Undefined => false,
            _ => true
        };
    }

    private Value EvalAssignment(AssignmentExpressionNode node, Environment env)
    {
        var value = Eval(node.Right!, env);
        if (node.Left is IdentifierNode ident)
        {
            env.Assign(ident.Name, value);
        }
        else
        {
            throw new NotImplementedException("Assignment to non-identifier not implemented");
        }
        return value;
    }

    private Value EvalProgram(ProgramNode node, Environment env)
    {
        Value result = Value.Undefined();
        foreach (var stmt in node.Statements)
        {
            result = Eval(stmt, env);
        }
        return result;
    }

    private Value EvalLet(LetStatementNode node, Environment env)
    {
        Value? value = null;
        if (node.Value != null)
        {
            value = Eval(node.Value, env);
        }
        env.Define(node.Name!.Name, value ?? Value.Undefined());
        return Value.Undefined();
    }
}
