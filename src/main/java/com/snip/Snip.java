package com.snip;

import java.io.File;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Scanner;
import java.io.FileNotFoundException;

public class Snip {
    private static final String readFile(String filePath) throws FileNotFoundException {
        File file = new File(filePath);
        if (!file.exists()) {
            throw new FileNotFoundException("File does not exist");
        }

        StringBuilder sb = new StringBuilder();
        try (Scanner scanner = new Scanner(file)) {
            while (scanner.hasNext()) {
                sb.append(scanner.nextLine());
            }
        }
        return sb.toString();
    }

    public static void main(String[] args) {
        HashMap<String, String> argsMap = ArgParser.parse(args);
        String inputFile = argsMap.get("input");

        try {
            String fileContent = readFile(inputFile);
            ArrayList<String> words = new ArrayList<>(); 

            for (String word : fileContent.split(" ")) {
                words.add(word);
            }

            System.out.println(words);
        } catch (FileNotFoundException e) {
            System.out.println("File not found");
            e.printStackTrace();
            System.exit(1);
        }
    }
}
