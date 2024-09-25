using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nanaspastries.Controllers
{
    public class BoletaViewModel
    {
        public int IdPedido { get; set; }
        public string Producto { get; set; }
        public float Precio { get; set; }
        public float Total { get; set; }
        public string Estado { get; set; }
    }
}