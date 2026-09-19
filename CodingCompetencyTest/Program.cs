  // 1)
  //
  // int n = 15;
  //
  // for (int x = 1; x <= n; x++)
  // {
  //     string output;
  //
  //     if (x % 15 == 0)
  //         output = "foobar";
  //     else if (x % 3 == 0)
  //         output = "foo";
  //     else if (x % 5 == 0)
  //         output = "bar";
  //     else
  //         output = x.ToString();
  //
  //     Console.Write(output);
  //
  //     if (x < n)
  //         Console.Write(", ");
  // }
  
  // 2)
  //
  // int n = 105;
  //
  // for (int x = 1; x <= n; x++)
  // {
  //     string output = "";
  //
  //     if (x % 3 == 0)
  //         output += "foo";
  //
  //     if (x % 5 == 0)
  //         output += "bar";
  //
  //     if (x % 7 == 0)
  //         output += "jazz";
  //
  //     if (output == "")
  //         output = x.ToString();
  //
  //     Console.Write(output);
  //
  //     if (x < n)
  //         Console.Write(", ");
  // }
  
  // 3)
  //
  // var rules = new (int Divisor, string Text)[]
  // {
  //     (3, "foo"),
  //     (4, "baz"),
  //     (5, "bar"),
  //     (7, "jazz"),
  //     (9, "huzz")
  // };
  //
  // int n = 15;
  //
  // for (int x = 1; x <= n; x++)
  // {
  //     string output = "";
  //
  //     foreach (var rule in rules)
  //     {
  //         if (x % rule.Divisor == 0)
  //             output += rule.Text;
  //     }
  //
  //     if (output == "")
  //         output = x.ToString();
  //
  //     Console.Write(output);
  //
  //     if (x < n)
  //         Console.Write(", ");
  // }
  
  // 4)
  //
  // using System;
  // using System.Collections.Generic;
  //
  // class NumberGenerator
  // {
  //     private readonly SortedDictionary<int, string> rules = new();
  //
  //     public void AddRule(int input, string output)
  //     {
  //         rules[input] = output;
  //     }
  //
  //     public void Print(int n)
  //     {
  //         for (int x = 1; x <= n; x++)
  //         {
  //             string output = "";
  //
  //             foreach (var rule in rules)
  //             {
  //                 if (x % rule.Key == 0)
  //                     output += rule.Value;
  //             }
  //
  //             if (output == "")
  //                 output = x.ToString();
  //
  //             Console.Write(output);
  //
  //             if (x < n)
  //                 Console.Write(", ");
  //         }
  //     }
  // }

  class Program
  { 
      static void Main()
      {
          var rules = new (int Divisor, string Text)[]
          {
              (3, "foo"),
              (4, "baz"),
              (5, "bar"),
              (7, "jazz"),
              (9, "huzz")
          };
  
          int n = 15;
  
          for (int x = 1; x <= n; x++)
          {
              string output = "";
  
              foreach (var rule in rules)
              {
                  if (x % rule.Divisor == 0)
                      output += rule.Text;
              }
  
              if (output == "")
                  output = x.ToString();
  
              Console.Write(output);
  
              if (x < n)
                  Console.Write(", ");
          }
          Console.WriteLine();
      }
  }
