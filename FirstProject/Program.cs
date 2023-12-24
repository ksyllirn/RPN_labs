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
            Console.WriteLine($"Reversed Polish Notation: {string.Join(" ", rpn)}");

            double result = Calculate(rpn);

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

        static int GetPriority(object operation)
        {
            switch (operation)
            {
                case '+': case '-': return 1;
                case '*': case '/': return 2;
                default: return 0;
            }
        }

        static float CalculateOperation(object operation, float firstNum, float secondNum)
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

        static List<object> ConvertToRPN(List<object> statement)
        {
            Stack<object> operations = new Stack<object>();
            List<object> result = new List<object>();

            foreach (object token in statement)
            {
                if (token is string)
                {
                    result.Add(token);
                }
                else if (token.Equals('+') || token.Equals('-') || token.Equals('*') || token.Equals('/'))
                {
                    while (operations.Count > 0 && GetPriority(operations.Peek()) >= GetPriority(token))
                    {
                        result.Add(operations.Pop());
                    }
                    operations.Push(token);
                }
                else if (token.Equals('('))
                {
                    operations.Push(token);
                }
                else if (token.Equals(')'))
                {
                    while (operations.Count > 0 && !operations.Peek().Equals('('))
                    {
                        result.Add(operations.Pop());
                    }
                    operations.Pop();
                }
            }

            while (operations.Count > 0)
            {
                result.Add(operations.Pop());
            }

            return result;
        }
        static float Calculate(List<object> StateInRPN)
        {
            for (int i = 0; i < StateInRPN.Count; i++)
            {
                if (StateInRPN[i] is char)
                {
                    float firstNumber = Convert.ToSingle(StateInRPN[i - 2]);
                    float secondNumber = Convert.ToSingle(StateInRPN[i - 1]);
                    float result = CalculateOperation(StateInRPN[i], firstNumber, secondNumber);
                    StateInRPN.RemoveRange(i - 2, 3);
                    StateInRPN.Insert(i - 2, result);
                    i -= 2;
                }
            }
            float CalculatedStatement = Convert.ToSingle(StateInRPN[0]);
            return CalculatedStatement;
        }
    }
}