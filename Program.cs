using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ZeidelMethod;

class ZeidelSolution
{
    private Matrix<double> A;
    private Vector<double> B;
    private Vector<double> X;
    private Vector<double> Xcopy;
    private int k;
    private double eps;
    private static char[] Indexes = {'\u2080', '\u2081', '\u2082', '\u2083', '\u2084', '\u2085', '\u2086', '\u2087', '\u2088', '\u2089'};
    
    public ZeidelSolution(int N, double eps)
    {
        this.eps = eps;
        var StringEps = Convert.ToString(eps);
        k = 10; //StringEps.Substring(StringEps.IndexOf('.') + 1).Length;
        this.A = ReadMatrix(N, N, "a");
        this.B = ReadVector(N, "b");
        X = DenseVector.OfVector(B) * 0;
        Xcopy = DenseVector.OfVector(B) * 0;
        for(var i = 0; i < N; i++)
            Console.WriteLine(XiExpression(i));
        Console.WriteLine(A);
    }

    public void CountSolution()
    {
        Console.WriteLine($"Шаг номер {0}:");
        for(var i = 0; i < X.Count; i++)
            Console.WriteLine(Xi(i));
        Console.WriteLine($"X{IntegerToIndex(1)} = {VectorToString(X, k)}");
        for(var seria_number = 1; CountDeltaI() >= eps; seria_number++)
        {
            Console.WriteLine($"Шаг номер {seria_number}:");
            X.CopyTo(Xcopy);
            for(var i = 0; i < X.Count; i++)
                Console.WriteLine(Xi(i));
            var Delta = CountDeltaI();
            Console.WriteLine($"\nX{IntegerToIndex(seria_number)} = {VectorToString(X, k)}");
            Console.WriteLine($"X{IntegerToIndex(seria_number - 1)} = {VectorToString(Xcopy, k)}");
            Console.WriteLine($"на шаге № {seria_number} \u03B4 = {Delta}\n");
        }
        Console.WriteLine($"\n\nРешение X = {VectorToString(X, k)}");
        Console.WriteLine($"Erv = {VectorToString(A * X - B)}");
    }

    private string Xi(int Index)
    {
        var Res = B.ElementAt(Index);
        var ResString = $"x{IntegerToIndex(Index)} = ({Math.Round(B.ElementAt(Index), k)} - (";
        for(var i = 0; i < X.Count; i++)
            if(i != Index)
            {
                 var Component = A.At(Index, i) * X.ElementAt(Index);
                 Res -= Component; 
                 var a = Math.Round(A.At(Index, i), k);
                 var x = Math.Round(X.ElementAt(i), k);
                 var SubResString = $"{a} * ";
                 if(a < 0.0)
                    SubResString = $"({a}) * ";
                 if(x < 0.0)
                     SubResString += $"({x}) + ";
                 else SubResString += $"{x} + ";
                 ResString += SubResString;
            }
        Res /= A.At(Index, Index);
        ResString = ResString[..^2] + $")) / {Math.Round(A.At(Index, Index), k)} = {Math.Round(Res, k)}";
        var Xnew = X.ToArray();
        Xnew[Index] = Res;
        X = DenseVector.OfArray(Xnew);
        return ResString;
    }

    private double CountDeltaI() => (X - Xcopy).AbsoluteMaximum();

    public static string IntegerToIndex(int Index)
    {
        var Res = "";
        foreach(var digit in Convert.ToString(Index).ToCharArray())
        {
            switch(digit)
            {
                case '0':
                    Res += Indexes[0];
                    break;
                case '1':
                    Res += Indexes[1];
                    break;
                case '2':
                    Res += Indexes[2];
                    break;
                case '3':
                    Res += Indexes[3];
                    break;
                case '4':
                    Res += Indexes[4];
                    break;
                case '5':
                    Res += Indexes[5];
                    break;
                case '6':
                    Res += Indexes[6];
                    break;
                case '7':
                    Res += Indexes[7];
                    break;
                case '8':
                    Res += Indexes[8];
                    break;
                case '9':
                    Res += Indexes[9];
                    break;
            }
        }
        return Res;
    }

    public string XiExpression(int index)
    {
        var Res = $"x{IntegerToIndex(index)} = (b{IntegerToIndex(index)} - (";
        for(var i = 0; i < B.Count; i++)
            if(i != index)
                Res += $"a{IntegerToIndex(index)} {IntegerToIndex(i)} * x{IntegerToIndex(i)} + ";
        Res = Res[..^2] + $")) / a{IntegerToIndex(index)} {IntegerToIndex(index)}";
        return Res;
    }

    public static Matrix<double> ReadMatrix(int NumOfCols, int NumOfStrs, string NameOfMatr = "A")
    {
        double[,] A = new double[NumOfStrs, NumOfCols];
        for(var i = 0; i < NumOfStrs; i++)
            for(var j = 0; j < NumOfCols; j++)
            {
                Console.Write($"Введите элемент {NameOfMatr}{IntegerToIndex(i)} {IntegerToIndex(j)}: ");
                A[i, j] = Convert.ToDouble(Console.ReadLine());
            }
        return DenseMatrix.OfArray(A);
    }
    public static Vector<double> ReadVector(int Length, string NameOfVector)
    {
        double[] V = new double[Length];
        for(var i = 0; i < Length; i++)
        {
            Console.Write($"Введите элемент {NameOfVector}{IntegerToIndex(i)}: ");
            V[i] = Convert.ToDouble(Console.ReadLine());
        }
        return DenseVector.OfArray(V);
    }

    public static string VectorToString(Vector<double> V, int k = 5)
    {
        var Res = "{";
        for(var i = 0; i < V.Count; i++)
            Res += $"{Math.Round(V.ElementAt(i), k)}; ";
        Res = Res[..^2] + '}';
        return Res;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Введите порядок матрицы системы: ");
        var N = Convert.ToInt32(Console.ReadLine());
        Console.Write("Введите точность \u03B5: ");
        var eps = Convert.ToDouble(Console.ReadLine());
        var ZS = new ZeidelSolution(N, eps);
        ZS.CountSolution();
    }
}
