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
public static class ProductTemperatureRequirements
{
    private static readonly Dictionary<string, double> RequiredTemperatures = new Dictionary<string, double>
    {
        { "Bananas", 13.3 },
        { "Chocolate", 18 },
        { "Fish", 2 },
        { "Meat", -15 },
        { "Ice cream", -18 },
        { "Frozen pizza", -30 },
        { "Cheese", 7.2 },
        { "Sausages", 5 },
        { "Butter", 20.5 },
        { "Eggs", 19 }
    };
    
    public static double GetRequiredTemperature(string product)
    {
        if (RequiredTemperatures.TryGetValue(product, out double temperature))
        {
            return temperature;
        }
        else
        {
            Console.WriteLine("Unknown product, temp set to 0");
            return 0;
        }
    }
}
public class ReeferContainer : Container, IHazardNotifier
{
    public double Temperature { get; private set; }

    public ReeferContainer(double maxCapacity, double weight, double height, double depth, double temperature, string productType)
        : base(maxCapacity, weight, height, depth, "R")
    {
        ProductType = productType;
        Temperature = ProductTemperatureRequirements.GetRequiredTemperature(productType);
    }

    public void NotifyHazard(string message)
    {
        throw new NotImplementedException();
    }
}

public class ContainerShip
{
    public string Name { get; private set; }
    public double MaxSpeed { get; private set; }
    public int MaxContainers { get; private set; }
    public double MaxWeight { get; private set; }
    private List<Container> containers;

    public ContainerShip(string name, double maxSpeed, int maxContainers, double maxWeight)
    {
        Name = name;
        MaxSpeed = maxSpeed;
        MaxContainers = maxContainers;
        MaxWeight = maxWeight;
        containers = new List<Container>();
    }

    public void LoadContainer(Container container)
    {
        if (containers.Count >= MaxContainers)
            throw new Exception("Too many containers");
        if ((GetTotalWeight() + container.Weight + container.CurrentLoad) / 1000 > MaxWeight)
            throw new OverfillException($"Max weight limit exceeded {MaxWeight}kg");
        containers.Add(container);
    }

    private double GetTotalWeight()
    {
        return containers.Sum(c => c.Weight + c.CurrentLoad);
    }
    
    public void UnloadContainer(string serialNumber)
    {
        containers.RemoveAll(c => c.SerialNumber == serialNumber);
    }
    
    public override string ToString()
    {
        return $"Ship: {Name} (Speed: {MaxSpeed} knots, Containers: {containers.Count}/{MaxContainers}, Weight: {GetTotalWeight()/1000}/{MaxWeight}t)";
    }
}
