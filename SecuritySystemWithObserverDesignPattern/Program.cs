namespace SecuritySystemWithObserverDesignPattern.ConsoleApplication;

/*
 * ExternalVisitor class representing an external visitor who enters/exits the company building
 */
public class ExternalVisitor
{
    public int ExternalVisitorId { get; set; }
    public string ExternalVisitorName { get; set; }
    public string PurposeOfVisit { get; set; }
    public DateTime EntryDateTime { get; set; }
    public DateTime ExitDateTime { get; set; }
    public bool IsInBuilding { get; set; }
    public int POCEmployeeId { get; set; }
}

/*
 * UnSubscriber class implements the built-in interface IDisposable. This is required for the observable's Subscribe method to be returned.
 * Using IDisposable object returned to observer from Subscribe method of observable, the observer can call Dispose method to unsubscribe from observable.
 * The constructor just accepts the total list of observers and current observer subscribing to the observable.
 * In the Dispose method implementation, we simply check if the observer is present in the list, if yes, we remove the observer from the list.
 * This indicates that, the observer is successfully removed from list of observers of the observable, results in unsubscribing from notifications.
 */
class UnSubscriber : IDisposable
{
    private List<IObserver<ExternalVisitor>> observers = null;
    private IObserver<ExternalVisitor> observer = null;
    public UnSubscriber(List<IObserver<ExternalVisitor>> observers, IObserver<ExternalVisitor> observer)
    {
        this.observers = observers;
        this.observer = observer;
    }
    public void Dispose()
    {
        if (this.observers.Contains(this.observer))
        {
            this.observers.Remove(observer);
        }
    }
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
     * We simply add the observer to the "observers" list instance variable on calling the Subscribe method on our observable.
     * We have to return an IDisposable so that, the observer can call the Dispose method defined in IDisposable interface, to unsubscribe from observable.
     */ 
    public IDisposable Subscribe(IObserver<ExternalVisitor> observer)
    {
        this.observers.Add(observer);
        IDisposable unsubscriber = new UnSubscriber(this.observers, observer);
        return unsubscriber;
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

    /*
     * ExternalVisitorExited method is called when an external visitor exited the company building.
     * To represent which visitor exited, we mandated 2 parameters the externalVisitorId which is unique id of a visitor and exited date time.
     * We are updating the ExitDateTime property and IsInBuilding property according to exit of the visitor and notifying all the observers.
     */
    public void ExternalVisitorExited(int externalVisitorId, DateTime exitedDateTime)
    {
        ExternalVisitor externalVisitor = this.externalVisitors.Where(e => e.ExternalVisitorId == externalVisitorId).FirstOrDefault();
        if(externalVisitor != null)
        {
            externalVisitor.ExitDateTime = exitedDateTime;
            externalVisitor.IsInBuilding = false;
            foreach(IObserver<ExternalVisitor> observer in this.observers)
            {
                observer.OnNext(externalVisitor);
            }
        }
    }

    /*
     * EndTheWorkDayOfCompany method is called to represent that company work time is over for the day.
     * We are just calling the OnComplete method of each observer, indicating that, the observable won't send any further notifications.
     */
    public void EndTheWorkDayOfCompany()
    {
        foreach(IObserver<ExternalVisitor> observer in this.observers)
        {
            observer.OnCompleted();
        }
    }
}

/*
 * The IEmployee interface to represent properties to be implemented for an employee.
 */
interface IEmployee
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string JobTitle { get; set; }
}

/*
 * The Employee class implementing IEmployee interface to represent a POC employee assigned to an external visitor.
 * The instance of Employee is used in the POCNotify observer class.
 */
class Employee : IEmployee
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string JobTitle { get; set; }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.ReadKey();
    }
}