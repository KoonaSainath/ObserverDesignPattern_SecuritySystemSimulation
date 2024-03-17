namespace SecuritySystemWithObserverDesignPattern.ConsoleApplication;

//ExternalVisitor class representing an external visitor who enters/exits the company building
class ExternalVisitor
{
    public int ExternalVisitorId { get; set; }
    public string ExternalVisitorName { get; set; }
    public string PurposeOfVisit { get; set; }
    public DateTime EntryDateTime { get; set; }
    public DateTime ExitDateTime { get; set; }
    public bool IsInBuilding { get; set; }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.ReadKey();
    }
}