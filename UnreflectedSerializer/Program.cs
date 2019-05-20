using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UnreflectedSerializer
{

    public class RootDescriptor<T>
    {
        private readonly string start = "<";
        private readonly string end = ">";
        private readonly string close = "/";

        public delegate void Serializer<U>(U instance, TextWriter writter);

        //public List<Serializer<T>> serializer;

        public Serializer<T> actions;        

        public void Serialize(TextWriter writer, T instance)
        {
            actions.Invoke(instance, writer);
        }

        public string EncapsulateAttribute(string attribute, bool closing = false)
        {
            if (closing)
                return start + close + attribute + end;
            return start + attribute + end;
        }

        public string EncValue(string attribute, string value, bool nested = false)
        {
            return EncapsulateAttribute(attribute) + value + EncapsulateAttribute(attribute, true);
        }

        public string EncValue(string attribute, int value, bool nested = false) =>
            EncValue(attribute, value.ToString(), nested);
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
            RootDescriptor<Person> rootDesc = GetPersonDescriptor();

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

        static RootDescriptor<Person> GetPersonDescriptor()
        {
            var rootDesc = new RootDescriptor<Person>();
            rootDesc.actions = (Person person, TextWriter writter) =>
            {
                writter.WriteLine(rootDesc.EncapsulateAttribute("Person"));
                writter.WriteLine(rootDesc.EncValue("FirstName", person.FirstName));
                writter.WriteLine(rootDesc.EncValue("LastName", person.LastName));
            };
            rootDesc.actions += (Person person, TextWriter writter) =>
            {
                var addressDesc = GetAddressDescriptor();
                writter.WriteLine(rootDesc.EncapsulateAttribute("HomeAddress"));
                addressDesc.actions(person.HomeAddress, writter);
                writter.WriteLine(rootDesc.EncapsulateAttribute("HomeAddress", true));
            };
            rootDesc.actions += (Person person, TextWriter writter) =>
            {
                var addressDesc = GetAddressDescriptor();
                writter.WriteLine(rootDesc.EncapsulateAttribute("WorkAddress"));
                addressDesc.actions(person.WorkAddress, writter);
                writter.WriteLine(rootDesc.EncapsulateAttribute("WorkAddress", true));
            };
            rootDesc.actions += (Person person, TextWriter writter) =>
            {
                var countryDesc = GetCountryDescriptor();
                writter.WriteLine(rootDesc.EncapsulateAttribute("CitizenOf"));
                countryDesc.actions(person.CitizenOf, writter);
                writter.WriteLine(rootDesc.EncapsulateAttribute("CitizenOf", true));
            };
            rootDesc.actions += (Person person, TextWriter writter) =>
            {
                var phoneDesc = GetPhoneNumberDescriptor();
                writter.WriteLine(rootDesc.EncapsulateAttribute("MobilePhone"));
                phoneDesc.actions(person.MobilePhone, writter);
                writter.WriteLine(rootDesc.EncapsulateAttribute("MobilePhone", true));
            };
            rootDesc.actions += (Person person, TextWriter writter) =>
            {
                writter.WriteLine(rootDesc.EncapsulateAttribute("Person", true));
            };

            return rootDesc;
        }

        static RootDescriptor<Address> GetAddressDescriptor()
        {
            var rootDesc = new RootDescriptor<Address>();
            rootDesc.actions = (Address address, TextWriter writter) =>
            {
                writter.WriteLine(rootDesc.EncValue("Street", address.Street));
                writter.WriteLine(rootDesc.EncValue("City", address.City));
            };

            return rootDesc;
        }
        static RootDescriptor<Country> GetCountryDescriptor()
        {
            var rootDesc = new RootDescriptor<Country>();
            rootDesc.actions = (Country country, TextWriter writter) =>
            {
                writter.WriteLine(rootDesc.EncValue("Name", country.Name));
                writter.WriteLine(rootDesc.EncValue("AreaCode", country.AreaCode));
            };

            return rootDesc;
        }

        static RootDescriptor<PhoneNumber> GetPhoneNumberDescriptor()
        {
            var rootDesc = new RootDescriptor<PhoneNumber>();
            rootDesc.actions = (PhoneNumber number, TextWriter writter) =>
            {
                var coutryDesc = GetCountryDescriptor();
                writter.WriteLine(rootDesc.EncapsulateAttribute("Country"));
                coutryDesc.actions(number.Country, writter);
                writter.WriteLine(rootDesc.EncapsulateAttribute("Country", true));
            };
            rootDesc.actions += (PhoneNumber phoneNumber, TextWriter writer) =>
            {
                writer.WriteLine( rootDesc.EncValue("Number", phoneNumber.Number));
            };

            return rootDesc;
        }
    }
}
