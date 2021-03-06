using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeriTabaniProje
{
    public class Person
    {
        public string id, firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType, cardStartDate, cardEndDate;
        
        public Person(string id, string firstName, string lastName, string age, string gender, string country, string city, string isTurkishCitizen, string registeredMuseum, string isCardActive, string cardType, string cardStartDate, string cardEndDate)
        {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.age = age;
            this.gender = gender;
            this.country = country;
            this.city = city;
            this.isTurkishCitizen = isTurkishCitizen;
            this.registeredMuseum = registeredMuseum;
            this.isCardActive = isCardActive;
            this.cardType = cardType;
            this.cardStartDate = cardStartDate;
            this.cardEndDate = cardEndDate;
        }

        public Person() { }
    
    }
}
