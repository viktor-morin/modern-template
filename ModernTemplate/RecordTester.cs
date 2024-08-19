namespace ModernTemplate
{
    public record ShowOne(string Name, uint Age);

    public static class TestRecord
    {
        public static void Test()
        {
            var record = new ShowOne("John", 30);
            var record2 = record with { Name = "Jane" };
        }
    }
}
