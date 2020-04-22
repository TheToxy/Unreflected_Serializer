using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UnreflectedSerializer
{
    public static class XML
    {
        /// <summary>
        /// Converts tagName to XML tag "element" => "<element>" 
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="closingTag"></param>
        /// <returns></returns>
        public static string ToTag(string tagName, bool closingTag = false) => closingTag ? $"</${tagName}>" : $"<${tagName}>";

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
        static class PersonDescriptor{
            public static readonly Dictionary<string, Action<Person, TextWriter, string>> fields = new Dictionary<string, Action<Person, TextWriter, string>>
            {
                { "FirstName", (person, writter, fieldName) => XML.SerializeElement(fieldName, person.FirstName, writter)},
                { "LastName" , (person, writter, fieldName) => XML.SerializeElement(fieldName, person.LastName, writter) },            
                { "HomeAddress", (p,w,fName) => GetAddressDescriptor(fName).Serialize(w, p.HomeAddress)},
                { "WorkAddress" , (p,w,fName) => GetAddressDescriptor(fName).Serialize(w, p.WorkAddress)},
                { "CitizenOf" , (p,w,fName) => GetCountryDescriptor(fName).Serialize(w, p.CitizenOf)},
                { "MobilePhone" , (p,w,fName) => GetPhoneNumberDescriptor(fName).Serialize(w, p.MobilePhone)}
            };
        }

        static class AddressDescriptor
        {
            public static readonly Dictionary<string, Action<Address, string, TextWriter>> fields = new Dictionary<string, Action<Address, string, TextWriter>>
            {
                { "Street" ,  (address, fName, writter) => XML.SerializeElement(fName, address.Street, writter) },
                { "City", (address, fName, writter) => XML.SerializeElement(fName, address.City, writter) },
            };          
        }

        static class CountryDescriptor
        {
            public static readonly Dictionary<string, Action<Country, string, TextWriter>> fields = new Dictionary<string, Action<Country, string, TextWriter>>
            {
                { "Name", (country, fName, writter) => XML.SerializeElement(fName, country.Name, writter) },
                { "AreaCode" ,  (country, fName, writter) => XML.SerializeElement(fName, country.AreaCode, writter) }
            };
        }

        static class PhoneNumberDescriptor
        {
            public static readonly Dictionary<string, Action<PhoneNumber, string, TextWriter>> fields = new Dictionary<string, Action<PhoneNumber, string, TextWriter>>
            {
                { "Country", (phoneNum, fName, writer) => GetCountryDescriptor(fName).Serialize(writer, phoneNum.Country) },
                { "Number" , (phoneNum, fName, writer) => XML.SerializeElement(fName, phoneNum.Number, writer) }
            };
        }

        static RootDescriptor<Person> GetPersonDescriptor(string elementName = "Person")
        {          
            var rootDesc = new RootDescriptor<Person>(elementName);
            rootDesc.actions = (Person person, TextWriter writter) =>
            {                
                foreach (var pair in PersonDescriptor.fields)
                {
                    pair.Value(person, writter, pair.Key);
                }              
            };
            return rootDesc;
        }

        static RootDescriptor<Address> GetAddressDescriptor(string elementName = "Address")
        {
            var rootDesc = new RootDescriptor<Address>(elementName);
            rootDesc.actions = (Address address, TextWriter writter) =>
            {
                foreach (var pair in AddressDescriptor.fields)
                {
                    pair.Value(address, pair.Key, writter);
                }
            };

            return rootDesc;
        }
       
        static RootDescriptor<Country> GetCountryDescriptor(string elementName = "Country")
        {
            var rootDesc = new RootDescriptor<Country>(elementName);
            rootDesc.actions = (Country country, TextWriter writter) =>
            {
                foreach (var pair in CountryDescriptor.fields)
                {
                    pair.Value(country, pair.Key, writter);
                }
            };
            return rootDesc;
        }

        static RootDescriptor<PhoneNumber> GetPhoneNumberDescriptor(string elementName = "Country")
        {
            var rootDesc = new RootDescriptor<PhoneNumber>(elementName);
            rootDesc.actions = (PhoneNumber phoneNumber, TextWriter writter) =>
            {
                foreach (var pair in PhoneNumberDescriptor.fields)
                {
                    pair.Value(phoneNumber, pair.Key, writter);
                }
            };
            return rootDesc;
        }
    }
}
