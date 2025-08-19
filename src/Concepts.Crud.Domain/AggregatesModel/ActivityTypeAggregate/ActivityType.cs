using Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;
using Concepts.Crud.Domain.SeedWork;

namespace Concepts.Crud.Domain.AggregatesModel.ActivityTypeAggregate;

public class ActivityType
    : Entity<Guid>, IAggregateRoot
{
    public string Name { get; private set; }
    
    public virtual ICollection<Activity> ActivityList { get; private set; }

    protected ActivityType()
    {
        ActivityList = new List<Activity>();
    }

    public ActivityType(string name): this()
    {
        Name = name;
        //// Add the OrderStarterDomainEvent to the domain events collection 
        // // to be raised/dispatched when committing changes into the Database [ After DbContext.SaveChanges() ]
        // AddOrderStartedDomainEvent(userId, userName, cardTypeId, cardNumber,
        //                             cardSecurityNumber, cardHolderName, cardExpiration);
    }
}