#include "globals.h"

const std::string token_to_string(const Token &tok) {
  switch (tok) {
  case Token::UNDEFINED:
    return "UNDEFINED";
  case Token::IF:
    return "IF";
  case Token::ELSE:
    return "ELSE";
  case Token::ELIF:
    return "ELIF";
  case Token::INT:
    return "INT";
  case Token::DOUBLE:
    return "DOUBLE";
  case Token::DOUBLEK:
    return "DOUBLEK";
  case Token::FUNCTION:
    return "FUNCTION";
  case Token::STRING:
    return "STRING";
  case Token::CHARK:
    return "CHARK";
  case Token::INTK:
    return "INTK";
  case Token::STRINGK:
    return "STRINGK";
  case Token::BOOLK:
    return "BOOLK";
  case Token::WHILE:
    return "WHILE";
  case Token::RIGHTPARENTHESIS:
    return "RIGHTPARENTHESIS";
  case Token::LEFTPARENTHESIS:
    return "LEFTPARENTHESIS";
  case Token::RIGHTBRACKET:
    return "RIGHTBRACKET";
  case Token::LEFTBRACKET:
    return "LEFTBRACKET";
  case Token::RIGHTBRACE:
    return "RIGHTBRACE";
  case Token::CHAR:
    return "CHAR";
  case Token::LEFTBRACE:
    return "LEFTBRACE";
  case Token::PERIOD:
    return "PERIOD";
  case Token::COMMA:
    return "COMMA";
  case Token::EQUAL:
    return "EQUAL";
  case Token::NOTEQUAL:
    return "NOTEQUAL";
  case Token::ASSIGN:
    return "ASSIGN";
  case Token::GREATERTHAN:
    return "GREATERTHAN";
  case Token::LESSERTHAN:
    return "LESSERTHAN";
  case Token::LESSTHANEQUAL:
    return "LESSTHANEQUAL";
  case Token::GREATERTHANEQUAL:
    return "GREATERTHANEQUAL";
  case Token::SEMICOLON:
    return "SEMICOLON";
  case Token::AND:
    return "AND";
  case Token::OR:
    return "OR";
  case Token::NOT:
    return "NOT";
  case Token::IDENTIFIER:
    return "IDENTIFIER";
  case Token::TRUEK:
    return "TRUE";
  case Token::FALSEK:
    return "FALSE";
  case Token::INVALID:
    return "INVALID";
  case Token::END:
    exit(0);
  case Token::NONE:
    return "NONE";
  case Token::START:
    return "START";
  default:
    return "no token, somehow";
  }
}

const std::string token_to_string(const ParserToken &tok) {
  switch (tok) {
  case ParserToken::UNDEFINED:
    return "UNDEFINED";
  case ParserToken::IF:
    return "IF";
  case ParserToken::ELSE:
    return "ELSE";
  case ParserToken::ELIF:
    return "ELIF";
  case ParserToken::INT:
    return "INT";
  case ParserToken::DOUBLE:
    return "DOUBLE";
  case ParserToken::DOUBLEK:
    return "DOUBLEK";
  case ParserToken::FUNCTION:
    return "FUNCTION";
  case ParserToken::STRING:
    return "STRING";
  case ParserToken::CHARK:
    return "CHARK";
  case ParserToken::INTK:
    return "INTK";
  case ParserToken::STRINGK:
    return "STRINGK";
  case ParserToken::BOOLK:
    return "BOOLK";
  case ParserToken::WHILE:
    return "WHILE";
  case ParserToken::RIGHTPARENTHESIS:
    return "RIGHTPARENTHESIS";
  case ParserToken::LEFTPARENTHESIS:
    return "LEFTPARENTHESIS";
  case ParserToken::RIGHTBRACKET:
    return "RIGHTBRACKET";
  case ParserToken::LEFTBRACKET:
    return "LEFTBRACKET";
  case ParserToken::RIGHTBRACE:
    return "RIGHTBRACE";
  case ParserToken::CHAR:
    return "CHAR";
  case ParserToken::LEFTBRACE:
    return "LEFTBRACE";
  case ParserToken::PERIOD:
    return "PERIOD";
  case ParserToken::COMMA:
    return "COMMA";
  case ParserToken::EQUAL:
    return "EQUAL";
  case ParserToken::NOTEQUAL:
    return "NOTEQUAL";
  case ParserToken::ASSIGN:
    return "ASSIGN";
  case ParserToken::GREATERTHAN:
    return "GREATERTHAN";
  case ParserToken::LESSERTHAN:
    return "LESSERTHAN";
  case ParserToken::LESSTHANEQUAL:
    return "LESSTHANEQUAL";
  case ParserToken::GREATERTHANEQUAL:
    return "GREATERTHANEQUAL";
  case ParserToken::SEMICOLON:
    return "SEMICOLON";
  case ParserToken::AND:
    return "AND";
  case ParserToken::OR:
    return "OR";
  case ParserToken::NOT:
    return "NOT";
  case ParserToken::IDENTIFIER:
    return "IDENTIFIER";
  case ParserToken::TRUEK:
    return "TRUE";
  case ParserToken::FALSEK:
    return "FALSE";
  case ParserToken::INVALID:
    return "INVALID";
  case ParserToken::END:
    exit(0);
  case ParserToken::NONE:
    return "NONE";
  case ParserToken::START:
    return "START";
  case ParserToken::IFSTMT:
    return "IFSTMT";
  case ParserToken::WHILESTMT:
    return "WHILESTMT";
  case ParserToken::DIGIT:
    return "DIGIT";
  case ParserToken::VARDECL:
    return "VARDECL";
  case ParserToken::TYPE:
    return "TYPE";
  case ParserToken::FNDECL:
    return "FNDECL";
  case ParserToken::BINOP:
    return "BINOP";
  case ParserToken::EXPR:
    return "EXPR";
  default:
    return "no token, somehow";
  }
}