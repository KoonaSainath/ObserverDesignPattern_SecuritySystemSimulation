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
    //The "externalVisitors" list acts as a data store that stores all external visitor objects who has entered the building.
    List<ExternalVisitor> externalVisitors = null;

    //The "observers" list stores all the observer objects which are subscribed to current observable
    List<IObserver<ExternalVisitor>> observers = null;
    public SecuritySystem()
    {
        this.externalVisitors = new List<ExternalVisitor>();
        this.observers = new List<IObserver<ExternalVisitor>>();
    }

    /*
     * The Subscribe method is defined in IObservable generic interface.
     * An observer calls this method to subscribe to this observable for notifications.
     * We simply add the observer to the "observers" list instance variable on calling the Subscribe method on our observable
     */ 
    public IDisposable Subscribe(IObserver<ExternalVisitor> observer)
    {
        throw new NotImplementedException();
    }

    /*
     * ExternalVisitorEntered method is called when an external visitor entered the building.
     * We are simply adding the externalVisitor object to the data store the observable is maintaining, i.e., the list "externalVisitors"
     * Then we are iterating through all available observers and sending notifications by calling "OnNext" method of each observer.
     * We pass the externalVisitor object to OnNext method. Understand that, "externalVisitor" object is the communicated data between observable & observer.
     */ 
    public void ExternalVisitorEntered(ExternalVisitor externalVisitor)
    {
        if (!this.externalVisitors.Contains(externalVisitor))
        {
            this.externalVisitors.Add(externalVisitor);
            foreach(IObserver<ExternalVisitor> observer in this.observers)
            {
                observer.OnNext(externalVisitor);
            }
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.ReadKey();
    }
}