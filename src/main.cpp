#include "lexer.h"
#include <cstring>
#include <fstream>
#include <iostream>
#include <sstream>
#include <string>
#include <string_view>
#include <vector>

std::string token_to_string(Token &tok) {
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
  default:
    return "no token, somehow";
  }
}

std::string readFile(const std::string &filename) {
  std::ifstream file(filename);
  if (!file.is_open()) {
    std::cerr << "Unable to open source file" << std::endl;
  }
  std::string source((std::istreambuf_iterator<char>(file)),
                     std::istreambuf_iterator<char>());
  // DEBUG:
  std::cout << source << std::endl;
  file.close();
  return source;
}

void writeFile(const std::string &filename, const std::stringstream &output) {
  std::ofstream file(filename);
  if (!file.is_open()) {
    std::cerr << "Unable to open output file" << std::endl;
  }
  file << output.str();
  file.close();
}

bool get_flags(const int &argc, char *argv[], std::string &filename,
               std::string &parseString, std::string& test_string) {
  if (argc == 2) { // input file
    filename = argv[1];
  } else if (argc == 3) { // -e flag "string to lex"
    if (std::strcmp(argv[1], "-e")) {
      parseString = argv[2];
    }
  }
  int ind = 0;
  char *currentFlag;
  char *currentOption;
  bool isTest{false};
  for (int count{0}; count < argc; count++) {
    if (count % 2) {
      currentFlag = argv[count];
    }
    else {
      if (strcmp(currentFlag, "-e") == 0 ) {
        parseString = argv[count];
      } else if (strcmp(currentFlag, "-t") == 0) {
        isTest = true;
        test_string = argv[count];
      } else {
        filename = argv[count];
      }
    }
  }
  return isTest;
}

int main(int argc, char *argv[]) {
  std::string filename = "./tests/lexer.snip"; // default filename
  std::string test_filename = "output.txt";
  std::string parse_string = "";
  bool isTest = get_flags(argc, argv, filename, parse_string, test_filename);
  std::unique_ptr<Token[]> token_stack = nullptr;
  std::string lines = readFile(filename);
  Lexer lex((parse_string.empty() ? lines : parse_string));
  lex.tokenize(token_stack);

  if (isTest) {
    std::stringstream ss; 
    int i{0};
    while (token_stack[i] != Token::END) {
      std::string s = token_to_string(token_stack[i]);
      ss << token_to_string(token_stack[i]) << std::endl;
      i++;
    }
    #include "../tests/test_lexer.h"
    writeFile(test_filename,ss);
  } else {
    int i{0};
    while (token_stack[i] != Token::END) {
      std::string s = token_to_string(token_stack[i]);
      std::cout << token_to_string(token_stack[i]) << std::endl;
      i++;
    }
  }
  return 0;
}
