using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;

namespace Server
{
    public class Query
    {
        public Person GetPerson() => new Person("Luke Skywalker");
    }

    public class Person
    {
        public Person(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public static ConcurrentBag<Person> Persons { get; } = new();
    }

    public class Mutation
    {
        public async Task<Person> AddPerson([Service] ITopicEventSender sender, Person person)
        {
            Person.Persons.Add(person);

            await sender.SendAsync(nameof(Subscription.PersonAdded), person);
            return person;
        }
    }

    public class Subscription
    {
        [Subscribe]
        public Person PersonAdded([EventMessage] Person person)
        {
            return person;
        }
    }
}