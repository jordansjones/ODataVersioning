using System;
using System.Linq;

namespace ODataVersioning.Models
{

    public class SimpleProduct : IEntity
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

    }

    public class CategorizedProduct : IEntity
    {


        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Category { get; set; }

    }
}
