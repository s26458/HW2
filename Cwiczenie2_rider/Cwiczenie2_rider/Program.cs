using System.Runtime.InteropServices;

public class OverfillException : Exception
{
    public OverfillException(string message) : base(message) { }
}

public interface IHazardNotifier
{
    void NotifyHazard(string message);
}

public abstract class Container
{
    public string SerialNumber { get; private set; }
    public double MaxCapacity { get; private set; } 
    public double CurrentLoad { get; protected set; } 
    public double Weight { get; private set; } 
    public double Height { get; private set; } 
    public double Depth { get; private set; } 
    protected string ProductType { get; set; }

    private static int serialCounter = 1;

    public Container(double maxCapacity, double weight, double height, double depth, string typeCode)
    {
        SerialNumber = $"KON-{typeCode}-{serialCounter++}";
        MaxCapacity = maxCapacity;
        Weight = weight;
        Height = height;
        Depth = depth;
        CurrentLoad = 0;
    }

    public virtual void Load(double mass)
    {
        if (mass + CurrentLoad > MaxCapacity)
            throw new OverfillException($"Cannot load {mass}kg. Exceeds max capacity of {MaxCapacity}kg");
        CurrentLoad += mass;
    }

    public virtual void Unload()
    {
        CurrentLoad = 0;
    }

    public override string ToString()
    {
        return $"Serial: {SerialNumber}, Load: {CurrentLoad}/{MaxCapacity}kg, Weight: {Weight}kg";
    }
}

public class LiquidContainer : Container, IHazardNotifier
{
    public bool IsHazardous {get; private set;}
    
    public LiquidContainer(double maxCapacity, double weight, double height, double depth, bool isHazardous) 
        : base(maxCapacity, weight, height, depth, "L")
    {
        IsHazardous = isHazardous;
        ProductType = isHazardous ? "Hazardous Liquid" : "Regular Liquid";
    }

    public override void Load(double mass)
    {
        double effectriveCapacity = IsHazardous ? MaxCapacity * 0.5 / MaxCapacity * 0.9;
        if (mass + CurrentLoad > effectriveCapacity)
            throw new OverfillException($"Cannot load {mass}kg it is more than {effectriveCapacity}kg.");
        base.Load(mass);
    }

    public void NotifyHazardous(string message)
    {
        Console.WriteLine($"HAZARD ALERT [{SerialNumber}]: {message}");
    }

    public void NotifyHazard(string message)
    {
        throw new NotImplementedException();
    }
}

public class GasContainer : Container, IHazardNotifier
{
    public double Pressure {get; private set;}

    public GasContainer(double maxCapacity, double weight, double height, double depth, double pressure)
        : base(maxCapacity, weight, height, depth, "G")
    {
        Pressure = pressure;
        ProductType = "Gas";
    }

    public override void Unload()
    {
        CurrentLoad = MaxCapacity * 0.05;
    }

    public void NotifyHazardous(string message)
    {
        Console.WriteLine($"HAZARD ALERT [{SerialNumber}]: {message}");
    }

    public void NotifyHazard(string message)
    {
        throw new NotImplementedException();
    }
}