namespace ModernTemplate;

//todo
public class ExampleClass
{
    private List<string> _list = new List<string>();

    public IEnumerable<string> GetListOld()
    {
        // old
        var newList = new List<string>();

        foreach (var item in _list)
        {
            newList.Add(item);
        }

        return newList;
    }

    //better memory usage
    public IEnumerable<string> GetListNew()
    {
        // old
        var newList = new List<string>();

        foreach (var item in _list)
        {
            yield return item;
        }
    }
}
