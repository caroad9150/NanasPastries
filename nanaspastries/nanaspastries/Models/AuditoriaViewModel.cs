using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nanaspastries.Models
{
    public class AuditoriaViewModel
    {
            public string Idcliente { get; set; }
            public DateTime FechaHoraLogin { get; set; }
            public string Ip { get; set; }
            public string Buscador { get; set; }
            public int? BloqueosClave { get; set; } 
            public string Accion { get; set; }

    }
}