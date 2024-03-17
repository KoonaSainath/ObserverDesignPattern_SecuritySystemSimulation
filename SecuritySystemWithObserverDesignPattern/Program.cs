namespace SecuritySystemWithObserverDesignPattern.ConsoleApplication;

//ExternalVisitor class representing an external visitor who enters/exits the company building
public class ExternalVisitor
{
    public int ExternalVisitorId { get; set; }
    public string ExternalVisitorName { get; set; }
    public string PurposeOfVisit { get; set; }
    public DateTime EntryDateTime { get; set; }
    public DateTime ExitDateTime { get; set; }
    public bool IsInBuilding { get; set; }
}

/*
 * SecuritySystem class performs the role as an observable (provider). 
 * Implements generic interface IObservable<ExternalVisitor> to get strongly typed with the ExternalVisitor
 * Subscribe(IObserver<ExternalVisitor> is the only method defined in IObservable interface, hence, this need to be implemented mandatorily
 * The Subscribe method need to return an object which can be referenced using IDisposable interface. This can be used by an observer to unsubscribe.
 */
class SecuritySystem : IObservable<ExternalVisitor>
{
    public IDisposable Subscribe(IObserver<ExternalVisitor> observer)
    {
        throw new NotImplementedException();
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.ReadKey();
    }
}