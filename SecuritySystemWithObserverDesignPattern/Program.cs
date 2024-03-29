﻿namespace SecuritySystemWithObserverDesignPattern.ConsoleApplication;

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

/*
 * The abstract class CustomObserver implements the generic IObserver interface of type ExternalVisitor.
 * OnCompleted, OnError, OnNext methods are abstracted because the POCNotify and SecurityStaffNotify observer classes will implement their own functionality.
 * The methods Subscribe and UnSubscribe have same implementation for both POCNotify and SecurityStaffNotify classes. Hence the implementation we provide in this abstract class can be utilized. This is implemented to follow the DRY principle (Donot Repeat Yourself).
 */
abstract class CustomObserver : IObserver<ExternalVisitor>
{
    protected IDisposable unsubscriber = null;
    protected List<ExternalVisitor> externalVisitors = null;
    public abstract void OnCompleted();

    public abstract void OnError(Exception error);

    public abstract void OnNext(ExternalVisitor value);

    public void Subscribe(IObservable<ExternalVisitor> securitySystemObservable)
    {
        unsubscriber = securitySystemObservable.Subscribe(this);
    }

    public void UnSubscribe()
    {
        this.unsubscriber.Dispose();
        this.externalVisitors.Clear();
    }
}

/*
 * The POCNotify class perform a role of one of the observer
 */
class POCNotify : CustomObserver
{
    IEmployee employee = null;
    public POCNotify(IEmployee employee)
    {
        this.externalVisitors = new List<ExternalVisitor>();
        this.employee = employee;
    }

    /*
     * Iterating through all external visiters assigned to current employee POC and displaying their details respectively as a report.
     */
    public override void OnCompleted()
    {
        Console.ResetColor();
        ConsoleStyle.SetConsoleStyle(ConsoleStyle.Style.POC);
        string heading = $"{this.employee.EmployeeName}'s daily visitor report";
        Console.WriteLine(heading);
        Console.WriteLine(new string('-', heading.Length));
        Console.WriteLine();
        foreach(ExternalVisitor visitor in this.externalVisitors)
        {
            if(visitor.POCEmployeeId == this.employee.EmployeeId)
            {
                Console.WriteLine($"{visitor.ExternalVisitorId.ToString().PadRight(visitor.ExternalVisitorId.ToString().Length + 10)}{visitor.ExternalVisitorName.PadRight(visitor.ExternalVisitorName.Length + 10)}Entry:{visitor.EntryDateTime.ToString("dd MMM yyyy hh:mm:ss tt").PadRight(visitor.EntryDateTime.ToString().Length + 10)}Exit:{visitor.ExitDateTime.ToString("dd MMM yyyy hh:mm:ss tt").PadRight(visitor.ExitDateTime.ToString().Length + 10)}");
                Console.WriteLine();
            }
        }
        Console.ResetColor();
    }

    /*
     * Just call the OnError method from observable class if any exception occurs. I haven't called anywhere for avoiding try catch blocks for simplicity.
     */
    public override void OnError(Exception error)
    {
        Console.WriteLine(error.Message);
        Console.WriteLine();
        Console.WriteLine(error.ToString());
    }

    /*
     * OnNext method is called by observable when an external visitor enters or exits the company building.
     * If the externalVisitor passed as parameter isn't available in this.externalVisitors list, that means the visitor is entering now.
     * When visitor is entering, we simply add the visitor object to this.externalVisitors list and print the entry notification.
     * If the externalVisitor passed as parameter is already present in this.externalVisitors list, that means visitor is exiting now.
     * When the visitor is exiting, we simply update the properties IsInBuilding and ExitDateTime of the external visitor object in the this.externalVisitors list appropriately and print exit notification.
     */
    public override void OnNext(ExternalVisitor externalVisitor)
    {
        ExternalVisitor visitor = this.externalVisitors.Where(e => e.ExternalVisitorId == externalVisitor.ExternalVisitorId).FirstOrDefault();

        //If visitor doesn't exist in this.externalVisitors, that means this is entry notification.
        if(visitor is null)
        {
            //To ensure we only notify the assigned POC employee about current external visitor's entry.
            if(externalVisitor.POCEmployeeId == this.employee.EmployeeId)
            {
                this.externalVisitors.Add(externalVisitor);
                Console.WriteLine();
                Console.ResetColor();
                ConsoleStyle.SetConsoleStyle(ConsoleStyle.Style.POC);
                Console.WriteLine($"Employee notification: Hey {this.employee.EmployeeName}! Your visitor {externalVisitor.ExternalVisitorName} has entered at {externalVisitor.EntryDateTime} for {externalVisitor.PurposeOfVisit.ToLower()}");
                Console.ResetColor();
                Console.WriteLine();
            }
        }
        //If visitor already exist in this.externalVisitors, that means this is exit notification.
        else
        {
            //To ensure we only notify the assigned POC employee about current external visitor's exit.
            if(externalVisitor.POCEmployeeId == this.employee.EmployeeId)
            {
                visitor.IsInBuilding = false;
                visitor.ExitDateTime = externalVisitor.ExitDateTime;
                Console.WriteLine();
                Console.ResetColor();
                ConsoleStyle.SetConsoleStyle(ConsoleStyle.Style.POC);
                Console.WriteLine($"Employee notification: Hey {this.employee.EmployeeName}! Your visitor {externalVisitor.ExternalVisitorName} has exited at {externalVisitor.ExitDateTime}");
                Console.ResetColor();
                Console.WriteLine();
            }
        }
    }

    public void Subscribe(IObservable<ExternalVisitor> securitySystemObservable)
    {
        unsubscriber = securitySystemObservable.Subscribe(this);
    }

    public void UnSubscribe()
    {
        unsubscriber.Dispose();
        this.externalVisitors.Clear();
    }
}

/*
 * The SecurityStaffNotify class performs the role of one of the observer
 */
class SecurityStaffNotify : CustomObserver
{
    public SecurityStaffNotify()
    {
        this.externalVisitors = new List<ExternalVisitor>();
    }

    /*
     * Iterating through all external visiters displaying their details respectively as a report.
     */
    public override void OnCompleted()
    {
        Console.ResetColor();
        ConsoleStyle.SetConsoleStyle(ConsoleStyle.Style.SecurityStaff);
        string heading = $"The security staff's daily visitor report";
        Console.WriteLine(heading);
        Console.WriteLine(new string('-', heading.Length));
        Console.WriteLine();
        foreach (ExternalVisitor visitor in this.externalVisitors)
        {
            Console.WriteLine($"{visitor.ExternalVisitorId.ToString().PadRight(visitor.ExternalVisitorId.ToString().Length + 10)}{visitor.ExternalVisitorName.PadRight(visitor.ExternalVisitorName.Length + 10)}Entry:{visitor.EntryDateTime.ToString("dd MMM yyyy hh:mm:ss tt").PadRight(visitor.EntryDateTime.ToString().Length + 10)}Exit:{visitor.ExitDateTime.ToString("dd MMM yyyy hh:mm:ss tt").PadRight(visitor.ExitDateTime.ToString().Length + 10)}");
            Console.WriteLine();
        }
        Console.ResetColor();
    }

    /*
     * Just call OnError method on observer object from observable class, when an exception occur, to execute exception handling code of observer specific.
     */
    public override void OnError(Exception error)
    {
        Console.WriteLine(error.Message);
        Console.WriteLine();
        Console.WriteLine(error.ToString());
    }

    /*
     * OnNext method is called when an external visitor is entering the building.
     * The SecurityStaffNotify is not a single entity like POCNotify. Hence, the security staff should be notified for all the external visitors.
     * For POCNotify, a specific employee POC would be assigned with few external visitors. So we notified only applicable POCs.
     */
    public override void OnNext(ExternalVisitor externalVisitor)
    {
        ExternalVisitor visitor = this.externalVisitors.Where(e => e.ExternalVisitorId == externalVisitor.ExternalVisitorId).FirstOrDefault();
        //If the externalVisitor is not available in this.externalVisitors list, that means the current visitor is entering the building
        if(visitor == null)
        {
            this.externalVisitors.Add(externalVisitor);
            Console.WriteLine();
            Console.ResetColor();
            ConsoleStyle.SetConsoleStyle(ConsoleStyle.Style.SecurityStaff);
            Console.WriteLine($"Employee notification: Hey security staff folks! The visitor {externalVisitor.ExternalVisitorName} has entered at {externalVisitor.EntryDateTime} for {externalVisitor.PurposeOfVisit.ToLower()}");
            Console.ResetColor();
            Console.WriteLine();
        }
        //If the externalVisitor is already available in this.externalVisitors list, that means the current visitor is exiting the building
        else
        {
            visitor.ExitDateTime = externalVisitor.ExitDateTime;
            visitor.IsInBuilding = false;
            Console.WriteLine();
            Console.ResetColor();
            ConsoleStyle.SetConsoleStyle(ConsoleStyle.Style.SecurityStaff);
            Console.WriteLine($"Employee notification: Hey security staff folks! The visitor {externalVisitor.ExternalVisitorName} has exited at {externalVisitor.ExitDateTime}");
            Console.ResetColor();
            Console.WriteLine();
        }
    }

    public void Subscribe(IObservable<ExternalVisitor> securitySystemObservable)
    {
        this.unsubscriber = securitySystemObservable.Subscribe(this);
    }

    public void UnSubscribe(IObservable<ExternalVisitor> securitySystemObservable)
    {
        unsubscriber.Dispose();
        this.externalVisitors.Clear();
    }
}

static class Factory<R,L> where L : class, R, new()
{
    public static R GetInstance()
    {
        R r;
        r = new L();
        return r;
    }
}

static class ProduceInstances
{
    public static IEmployee ProduceEmployeeInstance(int employeeId, string employeeName, string jobTitle)
    {
        IEmployee employee = Factory<IEmployee, Employee>.GetInstance();
        employee.EmployeeId = employeeId;
        employee.EmployeeName = employeeName;
        employee.JobTitle = jobTitle;
        return employee;
    }
    public static ExternalVisitor ProduceExternalVisitorInstance(int externalVisitorId, string externalVisitorName, bool isInBuilding, string purposeOfVisit, int pocEmployeeId, DateTime entryDateTime)
    {
        ExternalVisitor externalVisitor = Factory<ExternalVisitor, ExternalVisitor>.GetInstance();
        externalVisitor.ExternalVisitorId = externalVisitorId;
        externalVisitor.ExternalVisitorName = externalVisitorName;
        externalVisitor.IsInBuilding = isInBuilding;
        externalVisitor.PurposeOfVisit = purposeOfVisit;
        externalVisitor.POCEmployeeId = pocEmployeeId;
        externalVisitor.EntryDateTime = entryDateTime;
        return externalVisitor;
    }
}

static class ConsoleStyle
{
    public enum Style
    {
        POC,
        SecurityStaff,
        Default
    }

    public static void SetConsoleStyle(Style style)
    {
        switch (style)
        {
            case Style.POC:
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                break;
            case Style.SecurityStaff:
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                break;
            default:
                Console.ResetColor();
                break;
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        //The central observable is instantiated
        SecuritySystem observableSecuritySystem = new SecuritySystem();

        //Creating instance of poc empoyee-1 Jimmy
        IEmployee poc1 = ProduceInstances.ProduceEmployeeInstance(1, "Jimmy", "Auditor");

        //Creating instance of poc employee-2 Chuck
        IEmployee poc2 = ProduceInstances.ProduceEmployeeInstance(1, "Chuck", "Senior Auditor");

        //Observer-1 the POCNotify is instantiated for poc-1
        POCNotify observerPOC1 = new POCNotify(poc1);

        //Observer-2 the POCNotify is instantiated for poc-2
        POCNotify observerPOC2 = new POCNotify(poc2);

        //Observer-3 the SecurityStaffNotify is instantiated for Security Staff folks
        SecurityStaffNotify observerSecurityStaff = new SecurityStaffNotify();

        //Observer-1 subscribing to central observable
        observerPOC1.Subscribe(observableSecuritySystem);

        //Observer-2 subscribing to central obsersable
        observerPOC2.Subscribe(observableSecuritySystem);

        //Observer-3 subscribing to central observable
        observerSecurityStaff.Subscribe(observableSecuritySystem);

        //Creating instance of visitor1
        ExternalVisitor visitor1 = ProduceInstances.ProduceExternalVisitorInstance(10,"Kim",true,"Business meeting",1,DateTime.Now);

        //Creating instance of visitor2
        ExternalVisitor visitor2 = ProduceInstances.ProduceExternalVisitorInstance(20, "Howard", true, "Casual discussion", 1, DateTime.Now);

        //Creating instance of visitor3
        ExternalVisitor visitor3 = ProduceInstances.ProduceExternalVisitorInstance(30, "Walt", true, "Make a deal", 1, DateTime.Now);

        //Creating instance of visitor4
        ExternalVisitor visitor4 = ProduceInstances.ProduceExternalVisitorInstance(40, "Jessie", true, "Seeking help", 2, DateTime.Now);

        //Creating instance of visitor5
        ExternalVisitor visitor5 = ProduceInstances.ProduceExternalVisitorInstance(50, "Tuco", true, "Warning", 2, DateTime.Now);

        //Creating instance of visitor6
        ExternalVisitor visitor6 = ProduceInstances.ProduceExternalVisitorInstance(60, "Gus", true, "Start new business", 2, DateTime.Now);

        //Visitor1 enters the building
        observableSecuritySystem.ExternalVisitorEntered(visitor1);

        //Visitor2 enters the building
        observableSecuritySystem.ExternalVisitorEntered(visitor2);

        //Visitor1 exits the building
        observableSecuritySystem.ExternalVisitorExited(10, DateTime.Now);

        //Visitor3 enters the building
        observableSecuritySystem.ExternalVisitorEntered(visitor3);

        //Visitor4 enters the building
        observableSecuritySystem.ExternalVisitorEntered(visitor4);

        //Visitor 3 exits the building
        observableSecuritySystem.ExternalVisitorExited(30, DateTime.Now);

        //Visitor 5 enters the building
        observableSecuritySystem.ExternalVisitorEntered(visitor5);

        //Visitor 6 enters the building
        observableSecuritySystem.ExternalVisitorEntered(visitor6);

        //Visitor 2 exits the building
        observableSecuritySystem.ExternalVisitorExited(20, DateTime.Now);

        //Visitor 4 exits the building
        observableSecuritySystem.ExternalVisitorExited(40, DateTime.Now);

        //Visitor 5 exits the building
        observableSecuritySystem.ExternalVisitorExited(50, DateTime.Now);

        //Visitor 6 exits the building
        observableSecuritySystem.ExternalVisitorExited(60, DateTime.Now);

        //Completing the work day of the company to declare that no further notifications will be sent
        observableSecuritySystem.EndTheWorkDayOfCompany();

        Console.ReadKey();
    }
}