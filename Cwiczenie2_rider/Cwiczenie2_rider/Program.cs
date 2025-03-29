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