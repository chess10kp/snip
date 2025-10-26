using Xunit;
using Snip.Lexer;

namespace Tests;

public class LexerTests
{
    [Fact]
    public void Lexer_ShouldInitialize()
    {
        // Arrange
        var input = "5 + 3";
        
        // Act
        var lexer = new Lexer(input);
        
        // Assert
        Assert.NotNull(lexer);
    }

    [Fact]
    public void Tokenize_NumberTokens_ShouldReturnCorrectTokens()
    {
        // Arrange
        var input = "42 3.14 0";
        
        // Act
        var lexer = new Lexer(input);
        var tokens = lexer.Tokenize();
        
        // Assert
        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("42", tokens[0].Value);
        Assert.Equal(TokenType.Number, tokens[1].Type);
        Assert.Equal("3.14", tokens[1].Value);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal("0", tokens[2].Value);
        Assert.Equal(TokenType.EndOfFile, tokens[3].Type);
    }

    [Fact]
    public void Tokenize_StringTokens_ShouldReturnCorrectTokens()
    {
        // Arrange
        var input = "\"hello\" 'world'";
        
        // Act
        var lexer = new Lexer(input);
        var tokens = lexer.Tokenize();
        
        // Assert
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("hello", tokens[0].Value);
        Assert.Equal(TokenType.String, tokens[1].Type);
        Assert.Equal("world", tokens[1].Value);
        Assert.Equal(TokenType.EndOfFile, tokens[2].Type);
    }

    [Fact]
    public void Tokenize_BooleanTokens_ShouldReturnCorrectTokens()
    {
        // Arrange
        var input = "true false";
        
        // Act
        var lexer = new Lexer(input);
        var tokens = lexer.Tokenize();
        
        // Assert
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Boolean, tokens[0].Type);
        Assert.Equal("true", tokens[0].Value);
        Assert.Equal(TokenType.Boolean, tokens[1].Type);
        Assert.Equal("false", tokens[1].Value);
        Assert.Equal(TokenType.EndOfFile, tokens[2].Type);
    }

    [Fact]
    public void Tokenize_NullUndefinedTokens_ShouldReturnCorrectTokens()
    {
        // Arrange
        var input = "null undefined";
        
        // Act
        var lexer = new Lexer(input);
        var tokens = lexer.Tokenize();
        
        // Assert
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Null, tokens[0].Type);
        Assert.Equal("null", tokens[0].Value);
        Assert.Equal(TokenType.Undefined, tokens[1].Type);
        Assert.Equal("undefined", tokens[1].Value);
        Assert.Equal(TokenType.EndOfFile, tokens[2].Type);
    }

    [Fact]
    public void Tokenize_IdentifierTokens_ShouldReturnCorrectTokens()
    {
        // Arrange
        var input = "variable _private $camelCase";
        
        // Act
        var lexer = new Lexer(input);
        var tokens = lexer.Tokenize();
        
        // Assert
        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("variable", tokens[0].Value);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("_private", tokens[1].Value);
        Assert.Equal(TokenType.Identifier, tokens[2].Type);
        Assert.Equal("$camelCase", tokens[2].Value);
        Assert.Equal(TokenType.EndOfFile, tokens[3].Type);
    }

    [Theory]
    [InlineData("+", TokenType.Plus)]
    [InlineData("-", TokenType.Minus)]
    [InlineData("*", TokenType.Multiply)]
    [InlineData("/", TokenType.Divide)]
    [InlineData("=", TokenType.Assign)]
    [InlineData("==", TokenType.Equal)]
    [InlineData("!=", TokenType.NotEqual)]
    [InlineData("<", TokenType.LessThan)]
    [InlineData("<=", TokenType.LessThanOrEqual)]
    [InlineData(">", TokenType.GreaterThan)]
    [InlineData(">=", TokenType.GreaterThanOrEqual)]
    public void Tokenize_OperatorTokens_ShouldReturnCorrectTokens(string input, TokenType expectedType)
    {
        // Arrange & Act
        var lexer = new Lexer(input);
        var tokens = lexer.Tokenize();
        
        // Assert
        Assert.Equal(2, tokens.Count);
        Assert.Equal(expectedType, tokens[0].Type);
        Assert.Equal(input, tokens[0].Value);
        Assert.Equal(TokenType.EndOfFile, tokens[1].Type);
    }

    [Theory]
    [InlineData("(", TokenType.LeftParen)]
    [InlineData(")", TokenType.RightParen)]
    [InlineData("{", TokenType.LeftBrace)]
    [InlineData("}", TokenType.RightBrace)]
    [InlineData("[", TokenType.LeftBracket)]
    [InlineData("]", TokenType.RightBracket)]
    [InlineData(";", TokenType.Semicolon)]
    [InlineData(",", TokenType.Comma)]
    [InlineData(".", TokenType.Dot)]
    [InlineData(":", TokenType.Colon)]
    [InlineData("?", TokenType.QuestionMark)]
    public void Tokenize_PunctuationTokens_ShouldReturnCorrectTokens(string input, TokenType expectedType)
    {
        // Arrange & Act
        var lexer = new Lexer(input);
        var tokens = lexer.Tokenize();
        
        // Assert
        Assert.Equal(2, tokens.Count);
        Assert.Equal(expectedType, tokens[0].Type);
        Assert.Equal(input, tokens[0].Value);
        Assert.Equal(TokenType.EndOfFile, tokens[1].Type);
    }

    [Theory]
    [InlineData("let", TokenType.Let)]
    [InlineData("const", TokenType.Const)]
    [InlineData("var", TokenType.Var)]
    [InlineData("function", TokenType.Function)]
    [InlineData("return", TokenType.Return)]
    [InlineData("if", TokenType.If)]
    [InlineData("else", TokenType.Else)]
    [InlineData("for", TokenType.For)]
    [InlineData("while", TokenType.While)]
    [InlineData("do", TokenType.Do)]
    [InlineData("break", TokenType.Break)]
    [InlineData("continue", TokenType.Continue)]
    [InlineData("try", TokenType.Try)]
    [InlineData("catch", TokenType.Catch)]
    [InlineData("finally", TokenType.Finally)]
    [InlineData("throw", TokenType.Throw)]
    [InlineData("new", TokenType.New)]
    [InlineData("this", TokenType.This)]
    [InlineData("class", TokenType.Class)]
    [InlineData("import", TokenType.Import)]
    [InlineData("export", TokenType.Export)]
    [InlineData("from", TokenType.From)]
    [InlineData("as", TokenType.As)]
    [InlineData("type", TokenType.Type)]
    [InlineData("enum", TokenType.Enum)]
    [InlineData("public", TokenType.Public)]
    [InlineData("private", TokenType.Private)]
    [InlineData("protected", TokenType.Protected)]
    [InlineData("static", TokenType.Static)]
    [InlineData("async", TokenType.Async)]
    [InlineData("await", TokenType.Await)]
    [InlineData("typeof", TokenType.Typeof)]
    [InlineData("instanceof", TokenType.Instanceof)]
    [InlineData("in", TokenType.In)]
    [InlineData("of", TokenType.Of)]
    public void Tokenize_KeywordTokens_ShouldReturnCorrectTokens(string input, TokenType expectedType)
    {
        // Arrange & Act
        var lexer = new Lexer(input);
        var tokens = lexer.Tokenize();
        
        // Assert
        Assert.Equal(2, tokens.Count);
        Assert.Equal(expectedType, tokens[0].Type);
        Assert.Equal(input, tokens[0].Value);
        Assert.Equal(TokenType.EndOfFile, tokens[1].Type);
    }

    [Fact]
    public void Tokenize_ComplexExpression_ShouldReturnCorrectTokens()
    {
        // Arrange
        var input = "let x = 5 + 3;";
        
        // Act
        var lexer = new Lexer(input);
        var tokens = lexer.Tokenize();
        
        // Assert
        Assert.Equal(8, tokens.Count); // Include EndOfFile token
        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("x", tokens[1].Value);
        Assert.Equal(TokenType.Assign, tokens[2].Type);
        Assert.Equal(TokenType.Number, tokens[3].Type);
        Assert.Equal("5", tokens[3].Value);
        Assert.Equal(TokenType.Plus, tokens[4].Type);
        Assert.Equal(TokenType.Number, tokens[5].Type);
        Assert.Equal("3", tokens[5].Value);
        Assert.Equal(TokenType.Semicolon, tokens[6].Type);
        Assert.Equal(TokenType.EndOfFile, tokens[7].Type); // Added EndOfFile token
    }

    [Fact]
    public void Tokenize_EdgeCases_ShouldHandleEmptyInput()
    {
        // Arrange
        var input = "";
        
        // Act
        var lexer = new Lexer(input);
        var tokens = lexer.Tokenize();
        
        // Assert
        Assert.Single(tokens);
        Assert.Equal(TokenType.EndOfFile, tokens[0].Type);
    }
}