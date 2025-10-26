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
             VarStatementNode varStmt => EvalVar(varStmt, env),
             ConstStatementNode constStmt => EvalConst(constStmt, env),
             ExpressionStatementNode exprStmt => Eval(exprStmt.Expression, env),
            IfStatementNode ifStmt => EvalIf(ifStmt, env),
            ConditionalExpressionNode condExpr => EvalConditional(condExpr, env),
            BlockStatementNode blockStmt => EvalBlock(blockStmt, env),
             WhileStatementNode whileStmt => EvalWhile(whileStmt, env),
              ForStatementNode forStmt => EvalFor(forStmt, env),
              DoWhileStatementNode doWhileStmt => EvalDoWhile(doWhileStmt, env),
              SwitchStatementNode switchStmt => EvalSwitch(switchStmt, env),
              UnaryExpressionNode unaryExpr => EvalUnary(unaryExpr, env),
             CallExpressionNode callExpr => EvalCall(callExpr, env),
             NewExpressionNode newExpr => EvalNewExpression(newExpr, env),
             FunctionDeclarationNode funcDecl => EvalFunctionDeclaration(funcDecl, env),
             FunctionExpressionNode funcExpr => Value.Function(new FunctionValue(funcExpr.Parameters, funcExpr.Body, env)),
             ClassDeclarationNode classDecl => EvalClassDeclaration(classDecl, env),
             ThisExpressionNode => EvalThisExpression(env),
             SuperExpressionNode => EvalSuperExpression(env),
               ReturnStatementNode returnStmt => EvalReturn(returnStmt, env),
               BreakStatementNode breakStmt => EvalBreak(breakStmt, env),
               ContinueStatementNode continueStmt => EvalContinue(continueStmt, env),
               ThrowStatementNode throwStmt => EvalThrow(throwStmt, env),
               TryStatementNode tryStmt => EvalTry(tryStmt, env),
              MemberExpressionNode memberExpr => EvalMember(memberExpr, env),
             ObjectExpressionNode objExpr => EvalObject(objExpr, env),
             ArrayExpressionNode arrExpr => EvalArray(arrExpr, env),
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

    private Value ApplyBinaryOp(Value left, Value right, string op)
    {
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

    private Value EvalAdd(Value left, Value right)
    {
        if (left.Type == ValueType.String || right.Type == ValueType.String)
        {
            return Value.String((string)left.Data! + (string)right.Data!);
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
        // Handle numeric type coercion
        if ((left.Type == ValueType.Integer || left.Type == ValueType.Float) &&
            (right.Type == ValueType.Integer || right.Type == ValueType.Float))
        {
            return ToNumber(left) == ToNumber(right);
        }

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
        var rightValue = Eval(node.Right!, env);
        Value value;

        if (node.Left is IdentifierNode ident)
        {
            if (node.Operator == "=")
            {
                value = rightValue;
            }
            else
            {
                // Compound assignment: x += 1 becomes x = x + 1
                var leftValue = env.Get(ident.Name);
                var op = node.Operator.TrimEnd('='); // Remove = from end
                value = ApplyBinaryOp(leftValue, rightValue, op);
            }
            env.Assign(ident.Name, value);
        }
        else if (node.Left is MemberExpressionNode memberExpr)
        {
            var obj = Eval(memberExpr.Object, env);
            var propertyName = memberExpr.Computed
                ? ((string)Eval(memberExpr.Property, env).Data!)
                : ((IdentifierNode)memberExpr.Property).Name;

            if (node.Operator == "=")
            {
                value = rightValue;
            }
            else
            {
                // Compound assignment: obj.x += 1 becomes obj.x = obj.x + 1
                var leftValue = EvalMember(memberExpr, env);
                var op = node.Operator.TrimEnd('='); // Remove = from end
                value = ApplyBinaryOp(leftValue, rightValue, op);
            }

            // Set the property
            if (obj.Type == ValueType.Instance)
            {
                var instance = (ClassInstance)obj.Data!;
                instance.Properties[propertyName] = value;
            }
            else if (obj.Type == ValueType.Object)
            {
                var properties = (Dictionary<string, Value>)obj.Data!;
                properties[propertyName] = value;
            }
            else
            {
                throw new InvalidOperationException("Can only assign to properties on objects and instances");
            }
        }
        else
        {
            throw new NotImplementedException("Assignment to this expression type not implemented");
        }
        return value;
    }

    private Value EvalProgram(ProgramNode node, Environment env)
    {
        Value result = Value.Undefined();
        foreach (var stmt in node.Statements)
        {
            result = Eval(stmt, env);
            // If an exception is thrown at the top level, return it
            if (result.Type == ValueType.Exception)
            {
                return result;
            }
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

    private Value EvalVar(VarStatementNode node, Environment env)
    {
        Value? value = null;
        if (node.Value != null)
        {
            value = Eval(node.Value, env);
        }
        env.Define(node.Name.Name, value ?? Value.Undefined());
        return Value.Undefined();
    }

    private Value EvalConst(ConstStatementNode node, Environment env)
    {
        var value = Eval(node.Value, env);
        env.Define(node.Name.Name, value);
        return Value.Undefined();
    }

    private Value EvalIf(IfStatementNode node, Environment env)
    {
        var condition = Eval(node.Condition, env);
        if (IsTruthy(condition))
        {
            return EvalBlock(node.Consequence, env);
        }
        else if (node.Alternative != null)
        {
            return EvalBlock(node.Alternative, env);
        }
        return Value.Undefined();
    }

    private Value EvalConditional(ConditionalExpressionNode node, Environment env)
    {
        var test = Eval(node.Test, env);
        if (IsTruthy(test))
        {
            return Eval(node.Consequent, env);
        }
        else
        {
            return Eval(node.Alternative, env);
        }
    }

    private Value EvalBlock(BlockStatementNode node, Environment env)
    {
        var blockEnv = new Environment(env);
        Value result = Value.Undefined();
        foreach (var stmt in node.Body)
        {
            result = Eval(stmt, blockEnv);
            // If we encounter a control flow statement or exception, return it immediately
            if (result.Type == ValueType.Return || result.Type == ValueType.Break || result.Type == ValueType.Continue || result.Type == ValueType.Exception)
            {
                return result;
            }
        }
        return result;
    }

    private Value EvalWhile(WhileStatementNode node, Environment env)
    {
        Value result = Value.Undefined();
        while (IsTruthy(Eval(node.Condition, env)))
        {
            result = EvalBlock(node.Body, env);
            if (result.Type == ValueType.Break)
            {
                break;
            }
            if (result.Type == ValueType.Continue)
            {
                continue;
            }
            if (result.Type == ValueType.Exception)
            {
                return result;
            }
        }
        return result;
    }

    private Value EvalFor(ForStatementNode node, Environment env)
    {
        var loopEnv = new Environment(env);
        Value result = Value.Undefined();

        // Evaluate initializer
        if (node.Initializer != null)
        {
            Eval(node.Initializer, loopEnv);
        }

        // Loop
        while (node.Condition == null || IsTruthy(Eval(node.Condition, loopEnv)))
        {
            result = EvalBlock(node.Body, loopEnv);
            if (result.Type == ValueType.Break)
            {
                break;
            }
            if (result.Type == ValueType.Continue)
            {
                // Evaluate update before continuing
                if (node.Update != null)
                {
                    Eval(node.Update, loopEnv);
                }
                continue;
            }
            if (result.Type == ValueType.Exception)
            {
                return result;
            }

            // Evaluate update
            if (node.Update != null)
            {
                Eval(node.Update, loopEnv);
            }
        }

        return result;
    }

    private Value EvalDoWhile(DoWhileStatementNode node, Environment env)
    {
        Value result = Value.Undefined();
        do
        {
            result = EvalBlock(node.Body, env);
            if (result.Type == ValueType.Break)
            {
                break;
            }
            if (result.Type == ValueType.Continue)
            {
                continue;
            }
            if (result.Type == ValueType.Exception)
            {
                return result;
            }
        } while (IsTruthy(Eval(node.Condition, env)));
        return result;
    }

    private Value EvalSwitch(SwitchStatementNode node, Environment env)
    {
        var switchValue = Eval(node.Expression, env);
        Value result = Value.Undefined();
        bool matched = false;
        bool executing = false;
        bool broke = false;

        foreach (var caseNode in node.Cases)
        {
            if (!matched && caseNode.Test != null && ValuesEqual(Eval(caseNode.Test, env), switchValue))
            {
                matched = true;
                executing = true;
            }
            if (executing)
            {
                foreach (var stmt in caseNode.Consequent)
                {
                    var stmtResult = Eval(stmt, env);
                    if (stmtResult.Type == ValueType.Return)
                    {
                        return stmtResult;
                    }
                    if (stmtResult.Type == ValueType.Break)
                    {
                        broke = true;
                        break;
                    }
                    if (stmtResult.Type == ValueType.Exception)
                    {
                        return stmtResult;
                    }
                    result = stmtResult;
                }
                if (broke) break;
            }
        }

        if (!matched && node.DefaultCase != null && !broke)
        {
            foreach (var stmt in node.DefaultCase.Consequent)
            {
                var stmtResult = Eval(stmt, env);
                if (stmtResult.Type == ValueType.Return)
                {
                    return stmtResult;
                }
                if (stmtResult.Type == ValueType.Break)
                {
                    broke = true;
                    break;
                }
                if (stmtResult.Type == ValueType.Exception)
                {
                    return stmtResult;
                }
                result = stmtResult;
            }
        }

        return result;
    }

    private Value EvalUnary(UnaryExpressionNode node, Environment env)
    {
        var argument = Eval(node.Argument, env);

        return node.Operator switch
        {
            "!" => Value.Boolean(!IsTruthy(argument)),
            "-" => argument.Type switch
            {
                ValueType.Integer => Value.Integer(-(long)argument.Data!),
                ValueType.Float => Value.Float(-(double)argument.Data!),
                _ => throw new InvalidOperationException($"Cannot apply unary minus to {argument.Type}")
            },
            "+" => argument.Type switch
            {
                ValueType.Integer => argument,
                ValueType.Float => argument,
                _ => throw new InvalidOperationException($"Cannot apply unary plus to {argument.Type}")
            },
            _ => throw new NotImplementedException($"Unary operator {node.Operator} not implemented")
        };
    }

    private Value EvalCall(CallExpressionNode node, Environment env)
    {
        // Handle super() calls
        if (node.Callee is SuperExpressionNode)
        {
            return EvalSuperCall(node, env);
        }

        // Handle super.method() calls
        if (node.Callee is MemberExpressionNode memberExpr && memberExpr.Object is SuperExpressionNode)
        {
            return EvalSuperMethodCall(node, memberExpr, env);
        }

        var callee = Eval(node.Callee, env);
        if (callee.Type != ValueType.Function)
        {
            throw new InvalidOperationException("Can only call function values");
        }

        var function = (FunctionValue)callee.Data!;
        var args = node.Arguments.Select(arg => Eval(arg, env)).ToList();

        // Create function environment
        var functionEnv = new Environment(function.Closure);

        // If this is a method call (callee was a member expression), bind 'this'
        if (node.Callee is MemberExpressionNode memberExpr2)
        {
            var thisValue = Eval(memberExpr2.Object, env);
            functionEnv.Define("this", thisValue);
        }

        // Bind parameters
        for (int i = 0; i < function.Parameters.Count; i++)
        {
            var paramName = function.Parameters[i].Name.Name;
            var argValue = i < args.Count ? args[i] : Value.Undefined();
            functionEnv.Define(paramName, argValue);
        }

        // Execute function body
        Value result = Value.Undefined();
        foreach (var stmt in function.Body.Body)
        {
            result = Eval(stmt, functionEnv);
            if (result.Type == ValueType.Return)
            {
                return ((ReturnValue)result.Data!).Value;
            }
        }

        return result;
    }

    private Value EvalNewExpression(NewExpressionNode node, Environment env)
    {
        var classValue = Eval(node.Callee, env);
        if (classValue.Type != ValueType.Class)
        {
            throw new InvalidOperationException("Can only use 'new' with classes");
        }

        var classDef = (ClassValue)classValue.Data!;

        // Evaluate constructor arguments
        var args = new List<Value>();
        foreach (var arg in node.Arguments)
        {
            args.Add(Eval(arg, env));
        }

        // Create instance with properties from class members
        var properties = new Dictionary<string, Value>();

        // Initialize instance properties (non-static, non-method members)
        foreach (var member in classDef.Members)
        {
            if (!member.Value.IsStatic && member.Value.Value != null)
            {
                properties[member.Key] = member.Value.Value;
            }
        }

        var instance = new ClassInstance(classDef, properties);

        // Call constructor if it exists
        if (classDef.Members.TryGetValue("constructor", out var constructorMember) &&
            constructorMember.Value != null && constructorMember.Value.Type == ValueType.Function)
        {
            var constructor = (FunctionValue)constructorMember.Value.Data!;
            // Create a new environment for the constructor with 'this' bound
            var constructorEnv = new Environment(constructor.Closure);
            constructorEnv.Define("this", Value.Instance(instance));

            // Bind parameters
            for (int i = 0; i < constructor.Parameters.Count; i++)
            {
                var paramName = constructor.Parameters[i].Name.Name;
                var argValue = i < args.Count ? args[i] : Value.Undefined();
                constructorEnv.Define(paramName, argValue);
            }

            // Execute constructor body
            foreach (var stmt in constructor.Body.Body)
            {
                var result = Eval(stmt, constructorEnv);
                if (result.Type == ValueType.Return)
                {
                    break; // Constructors don't return values, but allow early return
                }
            }
        }

        return Value.Instance(instance);
    }

    private Value EvalFunctionDeclaration(FunctionDeclarationNode node, Environment env)
    {
        var function = new FunctionValue(node.Parameters, node.Body, env);
        env.Define(node.Name.Name, Value.Function(function));
        return Value.Undefined();
    }

    private Value EvalClassDeclaration(ClassDeclarationNode node, Environment env)
    {
        // Evaluate superclass if present
        ClassValue? superClass = null;
        if (node.SuperClass != null)
        {
            var superClassValue = env.Get(node.SuperClass.Name);
            if (superClassValue.Type != ValueType.Class)
            {
                throw new InvalidOperationException($"Cannot extend non-class {node.SuperClass.Name}");
            }
            superClass = (ClassValue)superClassValue.Data!;
        }

        // Create class members
        var members = new Dictionary<string, ClassMember>();
        foreach (var memberNode in node.Members)
        {
            Value? memberValue = null;
            if (memberNode.Value != null)
            {
                memberValue = Eval(memberNode.Value, env);
            }

            var member = new ClassMember(
                memberNode.Name,
                memberNode.Visibility,
                memberNode.IsStatic,
                memberNode.IsReadonly,
                memberValue
            );
            members[memberNode.Name] = member;
        }

        var classValue = new ClassValue(node.Name.Name, superClass, members, env);
        env.Define(node.Name.Name, Value.Class(classValue));
        return Value.Undefined();
    }

    private Value EvalThisExpression(Environment env)
    {
        try
        {
            return env.Get("this");
        }
        catch
        {
            throw new InvalidOperationException("'this' is not available in this context");
        }
    }

    private Value EvalSuperExpression(Environment env)
    {
        // 'super' by itself is invalid - it must be super() or super.method
        throw new InvalidOperationException("'super' must be used as super() or super.method");
    }

    private Value EvalReturn(ReturnStatementNode node, Environment env)
    {
        var value = node.Value != null ? Eval(node.Value, env) : Value.Undefined();
        return Value.Return(new ReturnValue(value));
    }

    private Value EvalBreak(BreakStatementNode node, Environment env)
    {
        return Value.Break(new BreakValue(Value.Undefined()));
    }

    private Value EvalContinue(ContinueStatementNode node, Environment env)
    {
        return Value.Continue(new ContinueValue(Value.Undefined()));
    }

    private Value EvalMember(MemberExpressionNode node, Environment env)
    {
        // Handle super.property access
        if (node.Object is SuperExpressionNode)
        {
            return EvalSuperMember(node, env);
        }

        var obj = Eval(node.Object, env);

        var propertyName = node.Computed
            ? ((string)Eval(node.Property, env).Data!)
            : ((IdentifierNode)node.Property).Name;

        if (obj.Type == ValueType.Instance)
        {
            var instance = (ClassInstance)obj.Data!;
            if (instance.Properties.TryGetValue(propertyName, out var value))
            {
                return value;
            }

            // Check class members (non-static)
            if (instance.Class.Members.TryGetValue(propertyName, out var member) && !member.IsStatic)
            {
                return member.Value ?? Value.Undefined();
            }

            // Check superclass chain
            var currentClass = instance.Class.SuperClass;
            while (currentClass != null)
            {
                if (currentClass.Members.TryGetValue(propertyName, out member) && !member.IsStatic)
                {
                    return member.Value ?? Value.Undefined();
                }
                currentClass = currentClass.SuperClass;
            }

            return Value.Undefined();
        }
        else if (obj.Type == ValueType.Object)
        {
            var properties = (Dictionary<string, Value>)obj.Data!;
            if (properties.TryGetValue(propertyName, out var value))
            {
                return value;
            }
            return Value.Undefined();
        }
        else
        {
            throw new InvalidOperationException("Can only access properties on objects and instances");
        }
    }

    private Value EvalSuperCall(CallExpressionNode node, Environment env)
    {
        // Get the current instance (this)
        var thisValue = env.Get("this");
        if (thisValue.Type != ValueType.Instance)
        {
            throw new InvalidOperationException("super() can only be called in constructors");
        }

        var instance = (ClassInstance)thisValue.Data!;
        var currentClass = instance.Class;

        // Find the superclass
        if (currentClass.SuperClass == null)
        {
            throw new InvalidOperationException("Cannot call super() - no superclass");
        }

        var superClass = currentClass.SuperClass;

        // Find the constructor in the superclass
        if (!superClass.Members.TryGetValue("constructor", out var constructorMember) ||
            constructorMember.Value == null || constructorMember.Value.Type != ValueType.Function)
        {
            // No constructor in superclass, just return
            return Value.Undefined();
        }

        var constructor = (FunctionValue)constructorMember.Value.Data!;
        var args = node.Arguments.Select(arg => Eval(arg, env)).ToList();

        // Create constructor environment with 'this' bound to the current instance
        var constructorEnv = new Environment(constructor.Closure);
        constructorEnv.Define("this", thisValue);

        // Bind parameters
        for (int i = 0; i < constructor.Parameters.Count; i++)
        {
            var paramName = constructor.Parameters[i].Name.Name;
            var argValue = i < args.Count ? args[i] : Value.Undefined();
            constructorEnv.Define(paramName, argValue);
        }

        // Execute constructor body
        foreach (var stmt in constructor.Body.Body)
        {
            var result = Eval(stmt, constructorEnv);
            if (result.Type == ValueType.Return)
            {
                break; // Constructors don't return values, but allow early return
            }
        }

        return Value.Undefined();
    }

    private Value EvalSuperMethodCall(CallExpressionNode node, MemberExpressionNode memberExpr, Environment env)
    {
        // Get the current instance (this)
        var thisValue = env.Get("this");
        if (thisValue.Type != ValueType.Instance)
        {
            throw new InvalidOperationException("super can only be used in class methods and constructors");
        }

        var instance = (ClassInstance)thisValue.Data!;
        var currentClass = instance.Class;

        // Find the superclass
        if (currentClass.SuperClass == null)
        {
            throw new InvalidOperationException("Cannot call super method - no superclass");
        }

        var propertyName = memberExpr.Computed
            ? ((string)Eval(memberExpr.Property, env).Data!)
            : ((IdentifierNode)memberExpr.Property).Name;

        // Find the method in the superclass
        var superClass = currentClass.SuperClass;
        while (superClass != null)
        {
            if (superClass.Members.TryGetValue(propertyName, out var member) && !member.IsStatic && member.Value != null && member.Value.Type == ValueType.Function)
            {
                var function = (FunctionValue)member.Value.Data!;
                var args = node.Arguments.Select(arg => Eval(arg, env)).ToList();

                // Create function environment with 'this' bound to the current instance
                var functionEnv = new Environment(function.Closure);
                functionEnv.Define("this", thisValue);

                // Bind parameters
                for (int i = 0; i < function.Parameters.Count; i++)
                {
                    var paramName = function.Parameters[i].Name.Name;
                    var argValue = i < args.Count ? args[i] : Value.Undefined();
                    functionEnv.Define(paramName, argValue);
                }

                // Execute function body
                Value result = Value.Undefined();
                foreach (var stmt in function.Body.Body)
                {
                    result = Eval(stmt, functionEnv);
                    if (result.Type == ValueType.Return)
                    {
                        return ((ReturnValue)result.Data!).Value;
                    }
                }

                return result;
            }
            superClass = superClass.SuperClass;
        }

        throw new InvalidOperationException($"Method {propertyName} not found in superclass");
    }

    private Value EvalSuperMember(MemberExpressionNode node, Environment env)
    {
        // Get the current instance (this)
        var thisValue = env.Get("this");
        if (thisValue.Type != ValueType.Instance)
        {
            throw new InvalidOperationException("super can only be used in class methods and constructors");
        }

        var instance = (ClassInstance)thisValue.Data!;
        var currentClass = instance.Class;

        // Find the superclass
        if (currentClass.SuperClass == null)
        {
            throw new InvalidOperationException("Cannot access super - no superclass");
        }

        var propertyName = node.Computed
            ? ((string)Eval(node.Property, env).Data!)
            : ((IdentifierNode)node.Property).Name;

        // Check superclass chain for the property/method
        var superClass = currentClass.SuperClass;
        while (superClass != null)
        {
            if (superClass.Members.TryGetValue(propertyName, out var member) && !member.IsStatic)
            {
                return member.Value ?? Value.Undefined();
            }
            superClass = superClass.SuperClass;
        }

        return Value.Undefined();
    }

    private Value EvalObject(ObjectExpressionNode node, Environment env)
    {
        var properties = new Dictionary<string, Value>();
        foreach (var prop in node.Properties)
        {
            // For now, assume non-computed properties only
            var key = prop.Key;
            var value = Eval(prop.Value, env);
            properties[key] = value;
        }
        return Value.Object(properties);
    }

    private Value EvalArray(ArrayExpressionNode node, Environment env)
    {
        var elements = new List<Value>();
        if (node.Elements != null)
        {
            foreach (var elem in node.Elements)
            {
                elements.Add(Eval(elem, env));
            }
        }
        return Value.Array(elements);
    }

    private Value EvalThrow(ThrowStatementNode node, Environment env)
    {
        var exceptionValue = Eval(node.Argument, env);
        return Value.Exception(new ExceptionValue(exceptionValue));
    }

    private Value EvalTry(TryStatementNode node, Environment env)
    {
        // Execute the try block
        Value result = EvalBlock(node.Block, env);

        // Check if an exception was thrown in the try block
        if (result.Type == ValueType.Exception)
        {
            var exceptionValue = (ExceptionValue)result.Data!;

            // If we have a catch handler
            if (node.Handler != null)
            {
                var catchEnv = new Environment(env);
                if (node.Handler.Parameter != null)
                {
                    // Bind the exception to the catch parameter
                    catchEnv.Define(node.Handler.Parameter.Name, exceptionValue.Value);
                }
                result = EvalBlock(node.Handler.Body, catchEnv);
            }
            else
            {
                // Re-throw the exception if no catch handler
                return result;
            }
        }

        // Execute finally block if present
        if (node.Finalizer != null)
        {
            EvalBlock(node.Finalizer, env);
        }

        return result;
    }
}
