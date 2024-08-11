namespace ModernTemplate.Domain.WorkoutAggregate;

public class Workout : Entity
{
    public WorkoutId Id { get; init; } = new WorkoutId(Guid.NewGuid());
    public string Name { get; private set; }
    public int NumberOfExcercises { get; private set; }

    private Workout(string name, int numberOfExcercises)
    {
        Name = name;
        NumberOfExcercises = numberOfExcercises;
    }

    public static Workout Create(string name, int numberOfExcercises)
    {
        return new Workout(name, numberOfExcercises);
    }
}
