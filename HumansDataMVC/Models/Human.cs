using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumansDataMVC.Models
{
    public class Human
    {
        public string fakeSSN { get; set; }
        public string legalFullName { get; set; }
        public string callByFirstName { get; set; }
        public string emailAddress { get; set; }
        public ICollection<Human> Children { get; set; }
        public ICollection<Pet> Pets { get; set; }
        public ICollection<Automobile> Cars { get; set; }
    }
}