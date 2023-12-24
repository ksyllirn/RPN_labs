using System.Linq.Expressions;

namespace RPNCalc
{
    internal class Program
    {
        static void Main()
        {
            Console.Write("Input mathematical statement: ");
            string statement = Console.ReadLine();

            List<Token> parsedStatement = Parse(statement);

            List<Token> rpn = ConvertToRPN(parsedStatement);

            double result = Calculate(rpn);
            Console.WriteLine($"Result: {result}");

        Console.ReadLine();
        }

        static List<Token> Parse(string statement)
        {
            List<Token> result = new();
            string number = "";

            foreach (char token in statement)
            {
                if (char.IsDigit(token) || token == ',')
                {
                    number += token;
                }
                else
                {
                    if (number != "")
                    {
                        Number num = new() { value = double.Parse(number) };
                        result.Add(num);
                        number = "";
                    }
                    if (token == '+' || token == '-' || token == '*' || token == '/')
                    {
                        Operation op = new() { symbol = token };
                        result.Add(op);
                    }
                    else if (token == '(' || token == ')')
                    {
                        Parenthesis bracket = new();
                        if (token == '(')
                        {
                            bracket.opening = true;
                        }
                        result.Add(bracket);
                    }
                }
            }

            if (number != "") 
            {
                Number num = new() { value = double.Parse(number) };
                result.Add(num);
            }

            return result;
        }

        static int GetPriority(Token operation)
        {
            if (operation is Operation op)
            {
                switch (op.symbol)
                {
                    case '+': case '-': return 1;
                    case '*': case '/': return 2;
                    default: return 0;
                }
            }
            return 0;
        }

        static double CalculateOperation(object operation, double firstNum, double secondNum)
        {
            switch (operation)
            {
                case '*': return firstNum * secondNum;
                case '/': return firstNum / secondNum;
                case '+': return firstNum + secondNum;
                case '-': return firstNum - secondNum;
                default: return 0;
            }
        }

        static List<Token> ConvertToRPN(List<Token> statement)
        {
            Stack<Token> operations = new();
            List<Token> result = new();

            foreach (Token token in statement)
            {
                if (token is Number number)
                {
                    result.Add(number);
                }
                else if (token is Operation operation)
                {
                    while (operations.Count > 0 && GetPriority(operations.Peek()) >= GetPriority(token))
                    {
                        result.Add(operations.Pop());
                    }
                    operations.Push(operation);
                }
                else if (token is Parenthesis bracket)
                {
                    if (bracket.opening)
                    {
                        operations.Push(bracket);
                    }
                    else
                    {
                        while (operations.Count > 0 && operations.Peek() is not Parenthesis)
                        {
                            result.Add(operations.Pop());
                        }
                        operations.Pop();
                    }
                }
            }

            while (operations.Count > 0)
            {
                result.Add(operations.Pop());
            }

            return result;
        }
        static double Calculate(List<Token> rpn)
        {
            Stack<double> numbers = new();

            foreach (Token token in rpn)
            {
                if (token is Number number)
                {
                    numbers.Push(number.value);
                }
                else if (token is Operation operation)
                {
                    double secondNum = numbers.Pop();
                    double firstNum = numbers.Pop();
                    char op = operation.symbol;
                    double resultedNum = CalculateOperation(op, firstNum, secondNum);
                    numbers.Push(resultedNum);
                }
            }
            return numbers.Pop();
        }
    }
}