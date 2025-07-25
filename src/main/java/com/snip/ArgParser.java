package com.snip;

import java.util.HashMap;

public class ArgParser {
    public static HashMap<String, String> parse(String[] args) {
        // -i <filename>
        HashMap<String, String> argsMap = new HashMap<>();
        for (int i = 0; i < args.length; i++) {
            if (args[i].equals("-i") || args[i].equals("-input")) {
                if (i + 1 < args.length) {
                    argsMap.put("input", args[i + 1]);
                }
            }
        }

        if (argsMap.get("input") == null) {
            System.out.println("Usage: snip -i <filename>");
            return null;
        }

        return argsMap;
    }
}
