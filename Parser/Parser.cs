using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Snip.Lexer;
using Snip.AST;
namespace Snip.Parser;

public class Parser
{
   private int _ptr = 0;
   private List<Token> _token { get; set; }

     // Precedence levels (higher number = higher precedence)
     private static readonly Dictionary<TokenType, int> _precedence = new() 
     {
        { TokenType.Assign, 0 }, // Assignment
        { TokenType.PlusAssign, 0 }, // Compound assignment
        { TokenType.MinusAssign, 0 }, // Compound assignment
        { TokenType.MultiplyAssign, 0 }, // Compound assignment
        { TokenType.DivideAssign, 0 }, // Compound assignment
        { TokenType.ModuloAssign, 0 }, // Compound assignment
{ TokenType.Plus, 1 },
         { TokenType.Minus, 1 },
         { TokenType.Multiply, 2 },
         { TokenType.Divide, 2 },
         { TokenType.Modulo, 2 },
         { TokenType.Power, 3 },
         { TokenType.LeftParen, 4 }, // Function calls
         { TokenType.Dot, 4 }, // Member access
         { TokenType.Equal, 5 },
         { TokenType.NotEqual, 5 },
         { TokenType.LessThan, 6 },
         { TokenType.LessThanOrEqual, 6 },
         { TokenType.GreaterThan, 6 },
         { TokenType.GreaterThanOrEqual, 6 },
      };

   public Parser(Lexer.Lexer lexer)
   {
      _token = lexer.Tokenize();
   }

   private Token? _peek(int lookahead = 0)
   {
      if (_ptr + lookahead < _token.Count) 
         return _token[_ptr + lookahead];
      return null;
   }

   public Token? _next()
   {
      var token = _peek();
      _ptr++;
      return token;
   }

   private int GetPrecedence(TokenType tokenType)
   {
      return _precedence.TryGetValue(tokenType, out var precedence) ? precedence : 0;
   }

   // Main Pratt parser entry point
   private ExpressionNode ParseExpression(int precedence = 0)
   {
      var left = ParsePrefixExpression();

      
       while (true)
       {
          var currentToken = _peek();
          if (currentToken == null) break;
          
// Semicolon, RightParen, LeftBrace, Colon, Comma, RightBracket, and RightBrace are not operators, they terminate expressions
           if (currentToken.Type == TokenType.Semicolon || currentToken.Type == TokenType.RightParen || currentToken.Type == TokenType.LeftBrace || currentToken.Type == TokenType.Colon || currentToken.Type == TokenType.Comma || currentToken.Type == TokenType.RightBracket || currentToken.Type == TokenType.RightBrace || currentToken.Type == TokenType.Newline) break;
           
           var currentPrecedence = GetPrecedence(currentToken.Type);
          // For right-associative operators (like assignment), use < instead of <=
          // This allows operators with same precedence to be processed
          if (currentPrecedence < precedence) break;
          
          left = ParseInfixExpression(left, currentToken.Type);
       }
      
      return left;
   }

    private ExpressionNode ParsePrefixExpression()
    {
       var token = _peek();
       if (token == null)
          throw new ParsingError("Expected expression");
          
       return token.Type switch
       {
          TokenType.Number => ParseNumberExpression(),
          TokenType.String => ParseStringExpression(),
          TokenType.Boolean => ParseBooleanExpression(),
          TokenType.Identifier => ParseIdentifierExpression(),
          TokenType.LeftParen => ParseParenthesizedExpression(),
          TokenType.LeftBrace => ParseObjectExpression(),
          TokenType.LeftBracket => ParseArrayExpression(),
          TokenType.Plus => ParseUnaryExpression(),
          TokenType.Minus => ParseUnaryExpression(),
          TokenType.New => ParseNewExpression(),
          TokenType.RightParen => throw new ParsingError($"Expected expression inside parens"),
          _ => throw new ParsingError($"ParsePrefixExpression: Unexpected token: {token.Type}")
       };
    }

     private ExpressionNode ParseInfixExpression(ExpressionNode left, TokenType operatorType)
     {
        return operatorType switch
        {
           TokenType.Assign => ParseAssignmentExpression(left, operatorType),
           TokenType.PlusAssign => ParseAssignmentExpression(left, operatorType),
           TokenType.MinusAssign => ParseAssignmentExpression(left, operatorType),
           TokenType.MultiplyAssign => ParseAssignmentExpression(left, operatorType),
           TokenType.DivideAssign => ParseAssignmentExpression(left, operatorType),
           TokenType.ModuloAssign => ParseAssignmentExpression(left, operatorType),
           TokenType.Plus => ParseBinaryExpression(left, operatorType),
           TokenType.Minus => ParseBinaryExpression(left, operatorType),
           TokenType.Multiply => ParseBinaryExpression(left, operatorType),
           TokenType.Divide => ParseBinaryExpression(left, operatorType),
           TokenType.Modulo => ParseBinaryExpression(left, operatorType),
TokenType.Power => ParseBinaryExpression(left, operatorType),
            TokenType.Equal => ParseBinaryExpression(left, operatorType),
            TokenType.NotEqual => ParseBinaryExpression(left, operatorType),
            TokenType.GreaterThan => ParseBinaryExpression(left, operatorType),
            TokenType.LessThan => ParseBinaryExpression(left, operatorType),
            TokenType.LessThanOrEqual => ParseBinaryExpression(left, operatorType),
            TokenType.GreaterThanOrEqual => ParseBinaryExpression(left, operatorType),
            TokenType.LeftParen => ParseCallExpression(left),
            TokenType.Dot => ParseMemberExpression(left),
           _ => throw new ParsingError($"Unexpected infix operator: {operatorType}")
        };
     }

     private ExpressionNode ParseAssignmentExpression(ExpressionNode left, TokenType operatorType)
     {
        _next(); // Consume the operator
        
        var precedence = GetPrecedence(operatorType);
        var right = ParseExpression(precedence);
        
var operatorString = operatorType switch
         {
            TokenType.Assign => "=",
            TokenType.PlusAssign => "+=",
            TokenType.MinusAssign => "-=",
            TokenType.MultiplyAssign => "*=",
            TokenType.DivideAssign => "/=",
            TokenType.ModuloAssign => "%=",
            _ => operatorType.ToString()
         };
         
         return new AssignmentExpressionNode
         {
            Left = left,
            Operator = operatorString,
            Right = right
         };
     }

     private ExpressionNode ParseBinaryExpression(ExpressionNode left, TokenType operatorType)
    {
       _next(); // Consume the operator
       
       var precedence = GetPrecedence(operatorType);
       var right = ParseExpression(precedence);
       
return operatorType switch
        {
           TokenType.Plus => new AddExpressionNode(left, right),
           TokenType.Minus => new SubtractExpressionNode(left, right),
           TokenType.Multiply => new MultiplyExpressionNode(left, right),
           TokenType.Divide => new DivideExpressionNode(left, right),
           TokenType.Modulo => new ModuloExpressionNode(left, right),
           TokenType.Power => new PowerExpressionNode(left, right),
TokenType.Equal => new EqualExpressionNode(left, right),
             TokenType.NotEqual => new NotEqualExpressionNode(left, right),
             TokenType.GreaterThan => new GreaterThanExpressionNode(left, right),
             TokenType.LessThan => new LessThanExpressionNode(left, right),
             TokenType.LessThanOrEqual => new LessThanOrEqualExpressionNode(left, right),
             TokenType.GreaterThanOrEqual => new GreaterThanOrEqualExpressionNode(left, right),
           _ => throw new ParsingError($"Unexpected binary operator: {operatorType}")
        };
    }

    private ExpressionNode ParseCallExpression(ExpressionNode callee)
    {
       _next(); // Consume '('
       var arguments = new List<ExpressionNode>();
       
       while (_peek() is not { Type: TokenType.RightParen })
       {
          var argument = ParseExpression();
          arguments.Add(argument);
          
          if (_peek()?.Type == TokenType.Comma)
          {
             _next(); // Consume comma
          }
       }
       
       if (_peek()?.Type != TokenType.RightParen)
          throw new ParsingError("Expected ')' in function call");
          
       _next(); // Consume ')'
       
       return new CallExpressionNode
       {
          Callee = callee,
          Arguments = arguments
       };
    }

    private ExpressionNode ParseMemberExpression(ExpressionNode obj)
    {
       _next(); // Consume '.'
       
       var propertyToken = _peek();
       if (propertyToken is not { Type: TokenType.Identifier })
          throw new ParsingError("Expected identifier after '.'");
          
       _next(); // Consume identifier
       
       var property = new IdentifierNode(propertyToken.Value);
       
       return new MemberExpressionNode(obj, property)
       {
          Computed = false
       };
    }

   private ExpressionNode ParseUnaryExpression()
   {
      var token = _peek();
      if (token?.Type != TokenType.Plus && token?.Type != TokenType.Minus)
         throw new ParsingError("Expected unary operator");
      
      _next(); // Consume the operator
      var argument = ParseExpression(GetPrecedence(TokenType.Plus)); // Unary operators have high precedence
      
      return new UnaryExpressionNode
      {
         Operator = token.Value,
         Argument = argument,
         IsPrefix = true
      };
   }

   private LetStatementNode ParseLetStatement()
   {
      // let name = expr
      var node = new LetStatementNode();
      var tok = _peek();
      if (tok == null || tok.Type != TokenType.Let)
         throw new ParsingError("Expected let");

      _next();
      tok = _peek();
      if (tok is not { Type: TokenType.Identifier })
         throw new ParsingError("Expected identifier");
      node.Name = new IdentifierNode(tok.Value);

      _next();
      tok = _peek();
      if (tok is not { Type: TokenType.Assign })
         throw new ParsingError("Expected =");

      _next();
      node.Value = ParseExpression();

      tok = _peek();
      if (tok is not { Type:  TokenType.Semicolon}) throw new ParsingError("Expected semicolon in assignment");
      _next();
      return node;
   }

   private StatementNode ParseReturnStatement()
   {
      var node = new ReturnStatementNode();
      if (_peek() is not { Type: TokenType.Return })
      {
         throw new ParsingError("Expected return statement");
      }

      _next();
      var tok = _peek();
      if (tok is { Type: TokenType.Semicolon })
      {
         _next();
         return node;
      }
      
node.Value = ParseExpression();
       
       if (_peek() is not {Type: TokenType.Semicolon}) throw new ParsingError("Expected semicolon in return");
       _next();
      return node;
   }

   private ExpressionNode ParseParenthesizedExpression()
   {
      _next(); // Consume '('
      var expression = ParseExpression();
      if (_peek()?.Type != TokenType.RightParen)
         throw new ParsingError("Expected ')'");
      _next(); // Consume ')'
      return expression;
   }

   private ExpressionNode ParseArrayExpression()
   {
      if (_peek()?.Type != TokenType.LeftBracket) throw new ParsingError("Expected ','");
      _next();
      var elements = new List<ExpressionNode>();
      
      while (_peek() is not { Type: TokenType.RightBracket })
      {
         var element = ParseExpression();
         elements.Add(element);
         
         if (_peek()?.Type == TokenType.Comma)
         {
            _next();
         }
      }
      
      _next(); 
      return new ArrayExpressionNode(elements);
   }

   private ExpressionNode ParseObjectExpression()
   {
      _next(); // Consume '{'
      var properties = new List<PropertyNode>();
      
      while (_peek() is not { Type: TokenType.RightBrace })
      {
         var property = ParseProperty();
         properties.Add(property);
         
         // Handle comma separator
         if (_peek()?.Type == TokenType.Comma)
         {
            _next(); // Consume comma
         }
      }
      
      _next(); // Consume '}'
      return new ObjectExpressionNode(properties);
   }

   private PropertyNode ParseProperty()
   {
      var tok = _peek();
      if (tok is not { Type: TokenType.String })
         throw new ParsingError("Expected string");
      _next();
      var key = tok.Value;
      
      // Handle colon separator
      if (_peek()?.Type == TokenType.Colon)
      {
         _next(); // Consume ':'
      }
      
      var value = ParseExpression();
      return new PropertyNode(key, value);
   }

   private IdentifierNode ParseIdentifierExpression()
   {
      var tok = _peek();
      if (tok is not { Type: TokenType.Identifier })
         throw new ParsingError("Expected identifier");
      _next();
      return new IdentifierNode(tok.Value);
   }

   private IntegerLiteralNode ParseNumberExpression()
   {
      var tok = _peek();
      if (tok is not { Type: TokenType.Number })
         throw new ParsingError("Expected number");
      _next();
      return new IntegerLiteralNode(tok.Value);
   }

   private StringLiteralNode ParseStringExpression()
   {
      var tok = _peek();
      if (tok is not { Type: TokenType.String })
         throw new ParsingError("Expected string");
      _next();
      return new StringLiteralNode(tok.Value);
   }

    private BooleanLiteralNode ParseBooleanExpression()
    {
       var tok = _peek();
       if (tok is not { Type: TokenType.Boolean })
          throw new ParsingError("Expected boolean");
       _next();
       return new BooleanLiteralNode(tok.Value == "true");
    }

    private ExpressionNode ParseNewExpression()
    {
       _next(); // Consume 'new'
       
        // Parse the constructor name (should be an identifier)
        var callee = ParseIdentifierExpression();
       
       if (_peek()?.Type != TokenType.LeftParen)
          throw new ParsingError("Expected '(' after new expression");
          
       _next(); // Consume '('
       var arguments = new List<ExpressionNode>();
       
       while (_peek() is not { Type: TokenType.RightParen })
       {
          var argument = ParseExpression();
          arguments.Add(argument);
          
          if (_peek()?.Type == TokenType.Comma)
          {
             _next(); // Consume comma
          }
       }
       
       if (_peek()?.Type != TokenType.RightParen)
          throw new ParsingError("Expected ')' in new expression");
          
       _next(); // Consume ')'
       
       return new NewExpressionNode
       {
          Callee = callee,
          Arguments = arguments
       };
    }

   private StatementNode? ParseNewLine()
   {
      var tok = _peek();
      if (tok is not { Type: TokenType.Newline })
      {
         throw new ParsingError("Expected newline");
      }
      _next();
      return null;
   }
   
   
   private StatementNode ParseContinueStatement()
   {
      var node =  new ContinueStatementNode();
      var tok = _peek();
      if (tok is not { Type: TokenType.Continue })
      {
         throw new ParsingError("Expected continue statement");
      }
      _next();
      tok = _peek();
      if (tok is null)
      {
         throw new ParsingError("Continue: Expected semicolon, found end of input");
      }
      if (tok is not { Type: TokenType.Semicolon })
      {
         throw new ParsingError($"Continue: Expected semicolon, found {tok.Type}");
      }
      _next();
      return node;
   }

   private IfStatementNode ParseIfStatement() {
      var node = new IfStatementNode();
      var tok = _peek();
      if (tok is not { Type: TokenType.If })
      {
         throw new ParsingError("Expected if");
      }
      _next();
      node.Condition = ParseExpression();
      tok = _peek();
      if (tok is not { Type: TokenType.LeftBrace })
      {
         throw new ParsingError("Expected {");
      }
      _next();
      tok = _peek();
      node.Consequence = new BlockStatementNode();
while (_peek() is not { Type: TokenType.RightBrace })
       {
         var currentTok = _peek();
         if (currentTok == null)
             throw new ParsingError("Expected }");

         node.Consequence.Body.Add(ParseStatement(currentTok));
       }
      _next();
      tok = _peek();
      if (tok is not { Type: TokenType.Else })
      {
         node.Alternative = null;
         return node;
      }
      _next();
      tok = _peek();
      if (tok is not { Type: TokenType.LeftBrace })
      {
         throw new ParsingError("Expected {");
      }
      _next();
      node.Alternative = new BlockStatementNode();
while (_peek() is not { Type: TokenType.RightBrace })
       {
          var currentTok = _peek();
          if (currentTok == null) break;
          node.Alternative.Body.Add(ParseStatement(currentTok));
       }
      _next();
      return node;
   }

private CaseNode ParseCase(bool isDefault = false) {
        var node = new CaseNode();
        var tok = _peek();
        if (!isDefault)
        {
            if (tok is not { Type: TokenType.Case })
            {
               throw new ParsingError("Expected case");
            }
            _next();
            node.Test = ParseExpression();
            tok = _peek();
            if (tok is not { Type: TokenType.Colon })
            {
               throw new ParsingError("Expected :");
            }
            _next();
        }
        else
        {
            // For default case, we should be on the Default token
            if (tok is not { Type: TokenType.Default })
            {
               throw new ParsingError("Expected default");
            }
            _next();
            tok = _peek();
            if (tok is not { Type: TokenType.Colon })
            {
               throw new ParsingError("Expected : after default");
            }
            _next();
        }
       node.Consequent = new List<StatementNode>();
       
       // Parse statements until we hit another case, default, or right brace
       while ((tok = _peek()) != null && 
              tok.Type != TokenType.Case && 
              tok.Type != TokenType.Default && 
              tok.Type != TokenType.RightBrace)
       {
          var statement = ParseStatement(tok);
          if (statement != null)
          {
             node.Consequent.Add(statement);
          }
          tok = _peek();
       }
       return node;
    }

   private StatementNode ParseSwitchStatement() {
      var node = new SwitchStatementNode(); 
      var tok = _peek();
      if (tok is not { Type: TokenType.Switch })
      {
         throw new ParsingError("Expected switch");
      }
      _next();
      node.Expression = ParseExpression();
      tok = _peek();
      if (tok is not { Type: TokenType.LeftBrace })
      {
         throw new ParsingError("Expected {");
      }
      _next();
node.Cases = new List<CaseNode>();
        while ((tok = _peek()) != null && tok.Type != TokenType.RightBrace)
        {
           // Skip whitespace and newlines
           if (tok.Type == TokenType.Newline || tok.Type == TokenType.Whitespace)
           {
              _next();
              continue;
           }
           
           if (tok.Type == TokenType.Case)
           {
              var caseNode = ParseCase();
              node.Cases.Add(caseNode);
           }
           else if (tok.Type == TokenType.Default)
           {
              node.DefaultCase = ParseCase(isDefault: true);
              tok = _peek();
              if (tok is not { Type: TokenType.RightBrace })
              {
                  throw new ParsingError("Expected } after default case");
              }
              break; // Exit the loop after handling default
           }
           else
           {
              throw new ParsingError($"Expected case or default in switch, got {tok.Type}");
           }
           tok = _peek();
        }
        
        // Consume the final RightBrace
        if (_peek()?.Type == TokenType.RightBrace)
        {
           _next();
        }
      return node;
   }

   private StatementNode ParseWhileStatement() {
      var node = new WhileStatementNode();
      var tok = _peek();
      if (tok is not { Type: TokenType.While })
      {
         throw new ParsingError("Expected while");
      }
      _next();
      node.Condition = ParseExpression();
      tok = _peek();
      if (tok is not { Type: TokenType.LeftBrace })
      {
         throw new ParsingError("Expected {");
      }
      _next();
      node.Body = new BlockStatementNode();
      while (tok is not { Type: TokenType.RightBrace } && tok is not null)
      {
         var statement = ParseStatement(tok);
         if (statement is null)
            throw new ParsingError("Expected statement");
         node.Body.Body.Add(statement);
         tok = _peek();
      }
      _next();
      return node;
   }

private StatementNode ParseBreakStatement()
    {
       var node =  new BreakStatementNode();
       var tok = _peek();
       if (tok is not { Type: TokenType.Break })
       {
          throw new ParsingError("Expected break");
       }

       _next();
       if (_peek() is not { Type: TokenType.Semicolon })
       {
          throw new ParsingError("Break: Expected semicolon");
       }

       _next();
       return node;
    }

    private StatementNode ParseBlockStatement()
    {
       var tok = _peek();
       if (tok is not { Type: TokenType.LeftBrace })
       {
          throw new ParsingError("Expected {");
       }

       _next(); // Consume '{'
       
       // Check if this looks like an object expression by looking ahead
       var nextTok = _peek();
       if (nextTok is { Type: TokenType.String } || nextTok is { Type: TokenType.RightBrace })
       {
          // This looks like an object expression, fall back to expression parsing
          // Put the token back so ParseExpressionStatement can handle it properly
          _ptr--;
          return ParseExpressionStatement();
       }
       
       var node = new BlockStatementNode();
       
       while ((_peek() is { Type: not TokenType.RightBrace }) && _peek() != null)
       {
          var currentTok = _peek();
          if (currentTok == null) break;
          
          var statement = ParseStatement(currentTok);
          if (statement != null)
          {
             node.Body.Add(statement);
          }
       }
       
       if (_peek() is not { Type: TokenType.RightBrace })
       {
          throw new ParsingError("Expected }");
       }
       
       _next(); // Consume '}'
       return node;
    }

   private StatementNode? ParseSemicolon()
   {
      _next();
      return null;
   }

   private StatementNode? ParseStatement(Token tok)
   {
      return tok.Type switch
      {
         TokenType.Let => ParseLetStatement(),
         TokenType.Return => ParseReturnStatement(),
         TokenType.Newline => ParseNewLine(),
         TokenType.Break => ParseBreakStatement(),
         TokenType.Continue => ParseContinueStatement(),
         TokenType.If => ParseIfStatement(),
         TokenType.While => ParseWhileStatement(),
         TokenType.Semicolon => ParseSemicolon(),
TokenType.Switch => ParseSwitchStatement(),
          TokenType.LeftBrace => ParseBlockStatement(),
          _ => ParseExpressionStatement()
      };
   }

   private ExpressionStatementNode ParseExpressionStatement()
   {
       var expression = ParseExpression();
       var node = new ExpressionStatementNode { Expression = expression };

       if (_peek()?.Type == TokenType.Semicolon)
       {
           _next();
       }

       return node;
   }

   public ProgramNode Parse()
   {
      var program = new ProgramNode();
      while (_peek() != null)
      {
         var tok = _peek();
         if (tok == null)
         {
            return program;
         }
         if (tok.Type == TokenType.EndOfFile) break;
         var node = ParseStatement(tok);
         if (node is null) continue; // handles new line
         program.Statements.Add(node);
      }
      return program;
   }

   public void PrintParseTree(ProgramNode program, int indent = 0)
   {
      PrintNode(program, indent);
   }

   private void PrintNode(AstNode node, int indent = 0)
   {
      if (node == null) return;

      string indentStr = new string(' ', indent * 2);
      string prefix = indent > 0 ? "├─ " : "";
      
      Console.WriteLine($"{indentStr}{prefix}{node.NodeType}{GetNodeDetails(node)}");

      switch (node)
      {
         case ProgramNode programNode:
            foreach (var statement in programNode.Statements)
            {
               PrintNode(statement, indent + 1);
            }
            break;

         case LetStatementNode letNode:
            if (letNode.Name != null)
               PrintNode(letNode.Name, indent + 1);
            if (letNode.Value != null)
               PrintNode(letNode.Value, indent + 1);
            break;

         case AddExpressionNode addNode:
            PrintNode(addNode.Left, indent + 1);
            PrintNode(addNode.Right, indent + 1);
            break;

         case SubtractExpressionNode subNode:
            PrintNode(subNode.Left, indent + 1);
            PrintNode(subNode.Right, indent + 1);
            break;

         case MultiplyExpressionNode mulNode:
            PrintNode(mulNode.Left, indent + 1);
            PrintNode(mulNode.Right, indent + 1);
            break;

         case DivideExpressionNode divNode:
            PrintNode(divNode.Left, indent + 1);
            PrintNode(divNode.Right, indent + 1);
            break;

         case ModuloExpressionNode modNode:
            PrintNode(modNode.Left, indent + 1);
            PrintNode(modNode.Right, indent + 1);
            break;

         case PowerExpressionNode powNode:
            PrintNode(powNode.Left, indent + 1);
            PrintNode(powNode.Right, indent + 1);
            break;

         case UnaryExpressionNode unaryNode:
            PrintNode(unaryNode.Argument, indent + 1);
            break;

         case ArrayExpressionNode arrayNode:
            if (arrayNode.Elements != null)
            {
               foreach (var element in arrayNode.Elements)
               {
                  PrintNode(element, indent + 1);
               }
            }
            break;

         case ObjectExpressionNode objNode:
            foreach (var property in objNode.Properties)
            {
               PrintNode(property, indent + 1);
            }
            break;

         case PropertyNode propNode:
            PrintNode(propNode.Value, indent + 1);
            break;

         case CallExpressionNode callNode:
            PrintNode(callNode.Callee, indent + 1);
            foreach (var arg in callNode.Arguments)
            {
               PrintNode(arg, indent + 1);
            }
            break;

         case MemberExpressionNode memberNode:
            PrintNode(memberNode.Object, indent + 1);
            PrintNode(memberNode.Property, indent + 1);
            break;

         case ConditionalExpressionNode condNode:
            PrintNode(condNode.Test, indent + 1);
            PrintNode(condNode.Consequent, indent + 1);
            PrintNode(condNode.Alternative, indent + 1);
            break;

         case AssignmentExpressionNode assignNode:
            PrintNode(assignNode.Left, indent + 1);
            PrintNode(assignNode.Right, indent + 1);
            break;

         case BinaryExpressionNode binaryNode:
            PrintNode(binaryNode.Left, indent + 1);
            PrintNode(binaryNode.Right, indent + 1);
            break;

         case FunctionExpressionNode funcNode:
            if (funcNode.Name != null)
               PrintNode(funcNode.Name, indent + 1);
            foreach (var param in funcNode.Parameters)
            {
               PrintNode(param, indent + 1);
            }
            PrintNode(funcNode.Body, indent + 1);
            break;

         case ArrowFunctionExpressionNode arrowNode:
            foreach (var param in arrowNode.Parameters)
            {
               PrintNode(param, indent + 1);
            }
            PrintNode(arrowNode.Body, indent + 1);
            break;

         case BlockStatementNode blockNode:
            foreach (var statement in blockNode.Body)
            {
               PrintNode(statement, indent + 1);
            }
            break;

         case IfStatementNode ifNode:
            PrintNode(ifNode.Condition, indent + 1);
            PrintNode(ifNode.Consequence, indent + 1);
            if (ifNode.Alternative != null)
               PrintNode(ifNode.Alternative, indent + 1);
            break;

         case WhileStatementNode whileNode:
            PrintNode(whileNode.Condition, indent + 1);
            PrintNode(whileNode.Body, indent + 1);
            break;

         case ForStatementNode forNode:
            if (forNode.Initializer != null)
               PrintNode(forNode.Initializer, indent + 1);
            if (forNode.Condition != null)
               PrintNode(forNode.Condition, indent + 1);
            if (forNode.Update != null)
               PrintNode(forNode.Update, indent + 1);
            PrintNode(forNode.Body, indent + 1);
            break;

         case ReturnStatementNode returnNode:
            if (returnNode.Value != null)
               PrintNode(returnNode.Value, indent + 1);
            break;

         case ExpressionStatementNode exprStmtNode:
            PrintNode(exprStmtNode.Expression, indent + 1);
            break;

         case FunctionDeclarationNode funcDeclNode:
            PrintNode(funcDeclNode.Name, indent + 1);
            foreach (var param in funcDeclNode.Parameters)
            {
               PrintNode(param, indent + 1);
            }
            PrintNode(funcDeclNode.Body, indent + 1);
            break;

         case ParameterNode paramNode:
            PrintNode(paramNode.Name, indent + 1);
            if (paramNode.DefaultValue != null)
               PrintNode(paramNode.DefaultValue, indent + 1);
            break;

         case ClassDeclarationNode classNode:
            PrintNode(classNode.Name, indent + 1);
            if (classNode.SuperClass != null)
               PrintNode(classNode.SuperClass, indent + 1);
            foreach (var member in classNode.Members)
            {
               PrintNode(member, indent + 1);
            }
            break;

         case ClassMemberNode memberDeclNode:
            if (memberDeclNode.Value != null)
               PrintNode(memberDeclNode.Value, indent + 1);
            break;

         case SwitchStatementNode switchNode:
            PrintNode(switchNode.Expression, indent + 1);
            foreach (var caseNode in switchNode.Cases)
            {
               PrintNode(caseNode, indent + 1);
            }
            if (switchNode.DefaultCase != null)
               PrintNode(switchNode.DefaultCase, indent + 1);
            break;

         case CaseNode caseNode:
            if (caseNode.Test != null)
               PrintNode(caseNode.Test, indent + 1);
            foreach (var stmt in caseNode.Consequent)
            {
               PrintNode(stmt, indent + 1);
            }
            break;

         case TryStatementNode tryNode:
            PrintNode(tryNode.Block, indent + 1);
            if (tryNode.Handler != null)
               PrintNode(tryNode.Handler, indent + 1);
            if (tryNode.Finalizer != null)
               PrintNode(tryNode.Finalizer, indent + 1);
            break;

         case CatchClauseNode catchNode:
            if (catchNode.Parameter != null)
               PrintNode(catchNode.Parameter, indent + 1);
            PrintNode(catchNode.Body, indent + 1);
            break;

         case TemplateLiteralNode templateNode:
            foreach (var quasi in templateNode.Quasis)
            {
               PrintNode(quasi, indent + 1);
            }
            foreach (var expr in templateNode.Expressions)
            {
               PrintNode(expr, indent + 1);
            }
            break;

         case ImportDeclarationNode importNode:
            foreach (var spec in importNode.Specifiers)
            {
               PrintNode(spec, indent + 1);
            }
            PrintNode(importNode.Source, indent + 1);
            break;

         case ImportSpecifierNode importSpecNode:
            PrintNode(importSpecNode.Local, indent + 1);
            if (importSpecNode.Imported != null)
               PrintNode(importSpecNode.Imported, indent + 1);
            break;

         case ExportDeclarationNode exportNode:
            PrintNode(exportNode.Declaration, indent + 1);
            break;

         case InterfaceDeclarationNode interfaceNode:
            PrintNode(interfaceNode.Name, indent + 1);
            foreach (var prop in interfaceNode.Properties)
            {
               PrintNode(prop, indent + 1);
            }
            foreach (var extend in interfaceNode.Extends)
            {
               PrintNode(extend, indent + 1);
            }
            break;
      }
   }

   private string GetNodeDetails(AstNode node)
   {
      return node switch
      {
         IntegerLiteralNode intNode => $" ({intNode.Value})",
         FloatLiteralNode floatNode => $" ({floatNode.Value})",
         StringLiteralNode strNode => $" (\"{strNode.Value}\")",
         BooleanLiteralNode boolNode => $" ({boolNode.Value})",
         IdentifierNode idNode => $" ({idNode.Name})",
         UnaryExpressionNode unaryNode => $" ({unaryNode.Operator})",
         BinaryExpressionNode binaryNode => $" ({binaryNode.Operator})",
         AssignmentExpressionNode assignNode => $" ({assignNode.Operator})",
         MemberExpressionNode memberNode => $" (computed: {memberNode.Computed})",
         PropertyNode propNode => $" (key: \"{propNode.Key}\", computed: {propNode.Computed}, shorthand: {propNode.Shorthand})",
         TemplateElementNode templateElementNode => $" (value: \"{templateElementNode.Value}\", tail: {templateElementNode.Tail})",
         ClassMemberNode classMemberNode => $" (name: {classMemberNode.Name}, visibility: {classMemberNode.Visibility}, static: {classMemberNode.IsStatic}, readonly: {classMemberNode.IsReadonly})",
         ParameterNode paramNode => $" (name: {paramNode.Name.Name}, type: {paramNode.TypeAnnotation ?? "none"})",
         FunctionDeclarationNode funcDeclNode => $" (name: {funcDeclNode.Name.Name}, async: {funcDeclNode.IsAsync}, returnType: {funcDeclNode.ReturnType ?? "none"})",
         ArrowFunctionExpressionNode arrowNode => $" (async: {arrowNode.IsAsync}, returnType: {arrowNode.ReturnType ?? "none"})",
         FunctionExpressionNode funcExprNode => $" (name: {funcExprNode.Name?.Name ?? "anonymous"}, async: {funcExprNode.IsAsync}, returnType: {funcExprNode.ReturnType ?? "none"})",
         _ => ""
      };
   }
}
