using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumansDataMVC.Models
{
    public enum ePetType
    {
        Cat,
        Dog
    }
    public class Pet
    {
        public ePetType typeOfPet { get; set; }
        public string breedOfPet { get; set; }
        public string colorOfPet { get; set; }
        public bool hasRabiesVaccination { get; set; }
    }
}