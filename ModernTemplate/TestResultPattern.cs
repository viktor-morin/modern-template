using CSharpFunctionalExtensions;

namespace ModernTemplate;

public class TestResultPattern
{

    public Result<string> ReturnString()
    {
        var i = 0;
        if (i == 0)
        {

           return Result.Failure<string>("");
        }

        return new SuccessResult("Success");
    }

    public void TestF()
    {
        var result = ReturnString();

        result.Match(
            s => Console.WriteLine(s),
            e => Console.WriteLine(e));
    }
}
