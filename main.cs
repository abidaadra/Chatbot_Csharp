using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Chatbot {
    static int Main() {
        if (!File.Exists("QA.txt")) {
            Console.WriteLine("QA.txt file not found.");
            return 1;
        }

        try {
            string[] allLines = File.ReadAllLines("QA.txt");
            List<QA> qaList = new List<QA>();

            foreach (string line in allLines) {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                    continue;

                string[] parts = line.Split('%');
                if (parts.Length == 2) {
                    qaList.Add(new QA { Question = parts[0], Answer = parts[1] });
                } else {
                    Console.WriteLine("Invalid line in QA.txt: " + line);
                }
            }

            while (true) {
                Console.Write("Your question: ");
                string question = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(question))
                    break;

                var similarQuestions = qaList.Select(qa => new { Question = qa.Question, Similarity = LevenshteinDistance.Compute(question, qa.Question) });
                var mostSimilarQuestion = similarQuestions.OrderBy(sq => sq.Similarity).FirstOrDefault();

                if (mostSimilarQuestion != null && mostSimilarQuestion.Similarity < 3) {
                    Console.WriteLine("Your question is similar to: " + mostSimilarQuestion.Question);
                    Console.WriteLine("Answer: " + qaList.First(qa => qa.Question == mostSimilarQuestion.Question).Answer);
                } else {
                    Console.WriteLine("Sorry, I couldn't find a suitable answer to your question.");
                }
            }

            return 0;
        } catch (Exception ex) {
            Console.WriteLine("An error occurred: " + ex.Message);
            return 1;
        }
    }
}

class QA {
    public string Question { get; set; }
    public string Answer { get; set; }
}

static class LevenshteinDistance {
    public static int Compute(string s, string t) {
        if (string.IsNullOrEmpty(s))
            return string.IsNullOrEmpty(t) ? 0 : t.Length;

        if (string.IsNullOrEmpty(t))
            return s.Length;

        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; d[i, 0] = i++) ;
        for (int j = 0; j <= m; d[0, j] = j++) ;

        for (int i = 1; i <= n; i++) {
            for (int j = 1; j <= m; j++) {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }
}
