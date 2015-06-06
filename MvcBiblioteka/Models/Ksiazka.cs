using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace MvcBiblioteka.Models
{
    public class Ksiazka
    {
        [JsonProperty(PropertyName="id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "tytul")]
        public string Tytul { get; set; }

        [JsonProperty(PropertyName = "autor")]
        public string Autor { get; set; }

        [JsonProperty(PropertyName = "wydawnictwo")]
        public string Wydawnictwo { get; set; }

        [JsonProperty(PropertyName = "miejsceWydania")]
        public string Miejsce_Wydania { get; set; }

        [JsonProperty(PropertyName = "rokWydania")]
        public string Rok_Wydania { get; set; }

        [JsonProperty(PropertyName = "isbn")]
        public string Nr_ISBN { get; set; }

        [JsonProperty(PropertyName = "ilosc")]
        public int Ilosc { get; set; }
    }
}