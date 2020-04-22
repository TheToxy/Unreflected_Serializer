using System;
using System.Collections.Generic;
using System.IO;

namespace UnreflectedSerializer
{
    public static class XML
    {
        public static string ToTag(string elementName, bool closing = false)
        {
            if (closing)
                return $"</${elementName}>";
            return $"<${elementName}>";
        }

        public static void SerializeElement<U>(string valueName, U value, TextWriter writer)
        {
            writer.WriteLine(XML.ToTag(valueName) + value + XML.ToTag(valueName, true));
        }
    }

    public class RootDescriptor<T>
    {
        private string rootElementName;
        public RootDescriptor(string rootElementName)
        {
            this.rootElementName = rootElementName;
        }

        public delegate void Serializer(T instance, TextWriter writter);
        public Serializer actions;

        public void Serialize(TextWriter writer, T instance)
        {
            writer.WriteLine(XML.ToTag(rootElementName));
            actions(instance, writer);
            writer.WriteLine(XML.ToTag(rootElementName, true));
        }
    }

    class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
    }

    class Country
    {
        public string Name { get; set; }
        public int AreaCode { get; set; }
    }

    class PhoneNumber
    {
        public Country Country { get; set; }
        public int Number { get; set; }
    }

    class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address HomeAddress { get; set; }
        public Address WorkAddress { get; set; }
        public Country CitizenOf { get; set; }
        public PhoneNumber MobilePhone { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            RootDescriptor<Person> rootDesc = GetGenericDescriptor<Person>("Person", FieldDescriptor.person);

            var czechRepublic = new Country { Name = "Czech Republic", AreaCode = 420 };
            var person = new Person
            {
                FirstName = "Pavel",
                LastName = "Jezek",
                HomeAddress = new Address { Street = "Patkova", City = "Prague" },
                WorkAddress = new Address { Street = "Malostranske namesti", City = "Prague" },
                CitizenOf = czechRepublic,
                MobilePhone = new PhoneNumber { Country = czechRepublic, Number = 123456789 }
            };

            rootDesc.Serialize(Console.Out, person);
        }
        
        static RootDescriptor<T> GetGenericDescriptor<T>(string elementName, Dictionary<string, Action<T, string, TextWriter>> fieldDescriptor)
        {
            var rootDesc = new RootDescriptor<T>(elementName);
            rootDesc.actions = (T instance, TextWriter writer) =>
            {
                foreach (var pair in fieldDescriptor)
                {
                    pair.Value(instance, pair.Key, writer);
                }
            };
            return rootDesc;
        }

        static class FieldDescriptor
        {
            public static readonly Dictionary<string, Action<Person, string, TextWriter>> person = new Dictionary<string, Action<Person, string, TextWriter>>
            {
                { "FirstName",      (person, fieldName, writer) => XML.SerializeElement(fieldName, person.FirstName, writer)},
                { "LastName" ,      (person, fieldName, writer) => XML.SerializeElement(fieldName, person.LastName, writer) },
                { "HomeAddress",    (person, fieldName, writer) => GetGenericDescriptor(fieldName, address).Serialize(writer, person.HomeAddress)},
                { "WorkAddress" ,   (person, fieldName, writer) => GetGenericDescriptor(fieldName, address).Serialize(writer, person.WorkAddress)},
                { "CitizenOf" ,     (person, fieldName, writer) => GetGenericDescriptor(fieldName, country).Serialize(writer, person.CitizenOf)},
                { "MobilePhone" ,   (person, fieldName, writer) => GetGenericDescriptor(fieldName, phoneNumber).Serialize(writer, person.MobilePhone)},
            };

            public static readonly Dictionary<string, Action<Address, string, TextWriter>> address = new Dictionary<string, Action<Address, string, TextWriter>>
            {
                { "Street" ,    (address, fName, writer) => XML.SerializeElement(fName, address.Street, writer) },
                { "City",       (address, fName, writer) => XML.SerializeElement(fName, address.City, writer) },
            };

            public static readonly Dictionary<string, Action<Country, string, TextWriter>> country = new Dictionary<string, Action<Country, string, TextWriter>>
            {
                { "Name",       (country, fName, writer) => XML.SerializeElement(fName, country.Name, writer) },
                { "AreaCode",   (country, fName, writer) => XML.SerializeElement(fName, country.AreaCode, writer) }
            };
            public static readonly Dictionary<string, Action<PhoneNumber, string, TextWriter>> phoneNumber = new Dictionary<string, Action<PhoneNumber, string, TextWriter>>
            {
                { "Country",    (phoneNum, fName, writer) => GetGenericDescriptor(fName, country).Serialize(writer, phoneNum.Country) },
                { "Number",     (phoneNum, fName, writer) => XML.SerializeElement(fName, phoneNum.Number, writer) }
            };
        }
    }
}
