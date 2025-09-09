using System.Diagnostics;
using Snip.Lexer;
using Snip.AST;
namespace Snip.Parser;

public class Parser
{
   private int _ptr = 0;
   private List<Token> _token { get; set; }
   public Parser(Lexer.Lexer lexer)
   {
      _token = lexer.Tokenize();
      Console.WriteLine(_token);
   }

   private Token? _peek(int lookahead = 0)
   {
      if (_ptr < _token.Count) return _token[_ptr+lookahead];
      Console.WriteLine("Peek called out of index");
      return null;

   }

   public Token? _next()
   {
      var token = _peek();
      _ptr++;
      return token;
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

      tok = _peek();
      if (tok is not { Type: TokenType.Assign })
         throw new ParsingError("Expected =");

      _next();
      node.Value = ParseExpression();
      return node;
   }

   private ExpressionNode ParseExpression() {
      var tok = _peek();
      if (tok is null)
         throw new ParsingError("Expected expression");
      return tok.Type switch
      {
         TokenType.Identifier => ParseIdentifierExpression(),
         TokenType.Number => ParseNumberExpression(),
         TokenType.String => ParseStringExpression(),
         TokenType.Boolean => ParseBooleanExpression(),
         TokenType.LeftParen => ParseParenthesizedExpression(),
         TokenType.LeftBrace => ParseObjectExpression(),
         TokenType.LeftBracket => ParseArrayExpression(),
         TokenType.Plus => ParseAddExpression(),
         TokenType.Minus => ParseSubtractExpression(),
         TokenType.Multiply => ParseMultiplyExpression(),
         TokenType.Divide => ParseDivideExpression(),
         TokenType.Modulo => ParseModuloExpression(),
         TokenType.Power => ParsePowerExpression(),
         _ => throw new ParsingError($"Unexpected token: {tok.Type}")
      };
   }

private ExpressionNode ParseMultiplyExpression() {
   var tok = _peek();
   if (tok is not { Type: TokenType.Multiply })
      throw new ParsingError("Expected *");
   _next();
   var left = ParseExpression();
   var right = ParseExpression();
   return new MultiplyExpressionNode(left, right);
}

private ExpressionNode ParseDivideExpression() {
   var tok = _peek();
   if (tok is not { Type: TokenType.Divide })
      throw new ParsingError("Expected /");
   _next();
   var left = ParseExpression();
   var right = ParseExpression();
   return new DivideExpressionNode(left, right);
}

private ExpressionNode ParseModuloExpression() {
   var tok = _peek();
   if (tok is not { Type: TokenType.Modulo })
      throw new ParsingError("Expected %");
   _next();
   var left = ParseExpression();
   var right = ParseExpression();
   return new ModuloExpressionNode(left, right);
}

private ExpressionNode ParseSubtractExpression() {
   var tok = _peek();
   if (tok is not { Type: TokenType.Minus })
      throw new ParsingError("Expected -");
   _next();
   var left = ParseExpression();
   var right = ParseExpression();
   return new SubtractExpressionNode(left, right);
}

private ExpressionNode ParseAddExpression() {
   var tok = _peek();
   if (tok is not { Type: TokenType.Plus })
      throw new ParsingError("Expected +");
   _next();
   var left = ParseExpression();
   var right = ParseExpression();
   return new AddExpressionNode(left, right);
}

private PropertyNode ParseProperty() {
   var tok = _peek();
   if (tok is not { Type: TokenType.String })
      throw new ParsingError("Expected string");
   _next();
   var key = tok.Value;
   var value = ParseExpression();
   return new PropertyNode(key, value);
}

private ExpressionNode ParseArrayExpression() {
   var tok = _peek();
   if (tok is not { Type: TokenType.LeftBracket })
      throw new ParsingError("Expected [");
   _next();
   var elements = new List<ExpressionNode>();
   while (_peek() is not { Type: TokenType.RightBracket }) {
      var element = ParseExpression();
      elements.Add(element);
   }
   _next();
   return new ArrayExpressionNode(elements);
}

private ExpressionNode ParseObjectExpression() {
   var tok = _peek();
   if (tok is not { Type: TokenType.LeftBrace })
      throw new ParsingError("Expected {");

   _next();
   var properties = new List<PropertyNode>();
   while (_peek() is not { Type: TokenType.RightBrace }) {
      var property = ParseProperty();
      properties.Add(property);
   }
   _next();
   return new ObjectExpressionNode(properties);
}

private ExpressionNode ParseParenthesizedExpression() {
   var tok = _peek();
   if (tok is not { Type: TokenType.LeftParen })
      throw new ParsingError("Expected (");
   _next();
   var expression = ParseExpression();
   tok = _peek();
   if (tok is not { Type: TokenType.RightParen })
      throw new ParsingError("Expected )");
   _next();
   return expression;
}

private BooleanLiteralNode ParseBooleanExpression() {
   var tok = _peek();
   if (tok is not { Type: TokenType.Boolean })
      throw new ParsingError("Expected boolean");

   _next();
   return new BooleanLiteralNode(tok.Value == "true");
}

private PowerExpressionNode ParsePowerExpression() {
   var tok = _peek();
   if (tok is not { Type: TokenType.Power })
      throw new ParsingError("Expected power");

   _next();
   var left = ParseExpression();
   var right = ParseExpression();
   return new PowerExpressionNode(left, right);
}

   private IdentifierNode ParseIdentifierExpression() {
      var tok = _peek();
      if (tok is not { Type: TokenType.Identifier })
         throw new ParsingError("Expected identifier");
      _next();
      return new IdentifierNode(tok.Value);
   }

   private IntegerLiteralNode ParseNumberExpression() {
      var tok = _peek();
      if (tok is not { Type: TokenType.Number })
         throw new ParsingError("Expected number");
      _next();
      return new IntegerLiteralNode(tok.Value);
   }

   private StringLiteralNode ParseStringExpression() {

      var tok = _peek();
      if (tok is not { Type: TokenType.String })
         throw new ParsingError("Expected string");
      _next();
      return new StringLiteralNode(tok.Value);
   }

   private StatementNode ParseStatement(Token tok)
   {
      return tok.Type switch
      {
         TokenType.Let => ParseLetStatement(),
         _ => throw new Exception($"Unexpected token: {tok.Type}")
      };
   }


   public ProgramNode parse() {
      Console.WriteLine("Parsing program");
      var program = new ProgramNode();
      while (_peek() != null) {
         var tok = _peek();
         if (tok == null) {
            return program;
         }
         var node = this.ParseStatement(tok);
         program.Statements.Add(node);
      }
      Console.WriteLine("Parsing program finished");
      return program;
   }

}