package com.snip;

import java.util.HashMap;

public class Lexer {
    enum TokenType { 
        NUMBER,
        PLUS,
        MINUS,
        MULTIPLY,
        DIVIDE,
        LPAREN,
        RPAREN, 
        MODULO,
        EQUALS,
        NOT_EQUAL,
        STRING,
        BOOLEAN,
        NULL,
        IDENTIFIER,
        KEYWORD,
        OPERATOR,
        PUNCTUATION,
        COMMENT,
        ERROR
     }
    public static HashMap<String, String> lex = new HashMap<>(); 

    public Lexer() {
        lex.put("let", "LET");
        lex.put("=", "EQUALS");
        lex.put("+", "PLUS");
        lex.put("-", "MINUS");
        lex.put("*", "MULTIPLY");
        lex.put("/", "DIVIDE");
        lex.put("%", "MODULO");
        lex.put("==", "EQUAL");
        lex.put("!=", "NOT_EQUAL");
        lex.put("and" ,"AND");
        lex.put("or", "OR");
        lex.put("not", "NOT");
        lex.put("if", "IF");
        lex.put("else", "ELSE");
        lex.put("while", "WHILE");
        lex.put("for", "FOR");
        lex.put("do", "DO");
        lex.put("break", "BREAK");
        lex.put("continue", "CONTINUE");
        lex.put("return", "RETURN");
        lex.put("true", "TRUE");
        lex.put("false", "FALSE");
        lex.put("null", "NULL");
    }

    public void lex(String[] input) {
        for (String tok: input) {
            if (lex.containsKey(tok)) {

            }
        }
    }
} 
