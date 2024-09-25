using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;

namespace nanaspastries.Models
{
    public class Carritorepos
    {
        public string IdPedido { get; set; }
        public string IdCliente { get; set; }
        public string Productos { get; set; }
        public string Cantidad { get; set; }
        public string Precio {get; set;}
        public string Total {get; set;}
        public string Estado {get; set;}
}
}
