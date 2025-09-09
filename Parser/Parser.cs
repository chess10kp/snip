using Snip.Lexer;
namespace Snip.Parser;

public class Parser
{
   private int _ptr = 0;
   private Lexer.Lexer _lexer { get; set; }
   private List<Token> _token { get; set; }
   public Parser(Lexer.Lexer lexer)
   {
      _lexer = lexer;
      _token = lexer.Tokenize();
   }

   public Token? peek()
   {
      if (_ptr < _token.Count)
      {
         Console.WriteLine("Peek called out of index");
         return null;
      }
      return _token[_ptr++];
   }

   public Token? _next()
   {
      var token = peek();
      _ptr++;
      return token;
   }
}
