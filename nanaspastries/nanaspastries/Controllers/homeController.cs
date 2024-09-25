using nanaspastries.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Claims;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Web;



namespace nanaspastries.Controllers
{
    public class homeController : Controller
    {
        public string Errores { get; set; }
        private static readonly HttpClient client = new HttpClient();
        // GET: home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Onb1()
        {
            return View();
        }
        public ActionResult Onb2()
        {
            return View();
        }
        public ActionResult Onb3()
        {
            return View();
        }
        public ActionResult Menu()  //elege si reposteria o caterin
        {
            return View();
        }
        public ActionResult Menurepos()  //todos los productos 
        {
            return View();
        }

        public ActionResult Menucate() //todos los paquetes 
        {
            return View();
        }
        [HttpPost]
        public ActionResult Menuadmin()  //cambiar precio, ver auditoria 
        {
            return View();
        }

        public ActionResult Acercade()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Loginn(string id, string clave, int contador = 0)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    clave = ReverseString(clave);
                    string query = "EXEC mantenclientes @op = 'login', @id = @id, @nombre = '', @cel = NULL, @correo = '', @direccion = '', @clave = @clave, @token = NULL, @tipo = '', @estado = '', @pregunta = '', @respuesta = '', @ip = @ip, @buscador = @buscador";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@clave", clave);
                        command.Parameters.AddWithValue("@ip", Request.UserHostAddress);
                        command.Parameters.AddWithValue("@buscador", Request.Browser.Browser);

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            Session["LoginAttempts"] = 0; // Reset login attempts on successful login
                            return View("Token");
                        }
                        else
                        {
                            contador = (int)(Session["LoginAttempts"] ?? 0);
                            contador++;
                            Session["LoginAttempts"] = contador;

                            if (contador >= 3)
                            {
                                BloquearUsuario(id);
                                var modelo = new MensajeCombinedViewModel
                                {
                                    Mensaje = "Su clave es incorrecta, su cuenta ha sido bloqueada. Le invitamos a pasar por el apartado de recuperación."
                                };
                                return View("mensaje", modelo);
                            }
                            else
                            {
                                var modelo = new MensajeCombinedViewModel
                                {
                                    Mensaje = "Su clave es incorrecta."
                                };
                                return View("mensaje", modelo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HttpStatusCodeResult(500, "Error en la base de datos");
            }
        }

        private void BloquearUsuario(string id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE clientes SET estado = 'b' WHERE id = @id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Ingreso(string id, string token)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "EXEC mantenclientes @op = 'token', @id = @id, @nombre = '', @cel = NULL, @correo = '', @direccion = '', @clave = '', @token = @token, @tipo = '', @estado = '', @pregunta = '', @respuesta = '', @ip = @ip, @buscador = @buscador";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@token", token);
                        command.Parameters.AddWithValue("@ip", Request.UserHostAddress);
                        command.Parameters.AddWithValue("@buscador", Request.Browser.Browser);

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            return View("Menu");
                        }
                        else
                        {
                            var modelo = new MensajeCombinedViewModel   
                            {
                                Mensaje = "Token invalido, intentelo de nuevo"
                            };
                            return View("mensaje", modelo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HttpStatusCodeResult(500, "Error en la base de datos");
            }
        }

        //admin 
        [HttpGet]
        public ActionResult LoginAdmin()
        {
            return View(); 
        }
        [HttpPost]
        public ActionResult adminlogin(string id, string clave, int contador = 0)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Invertir la clave antes de usarla en la consulta
                    clave = ReverseString(clave);

                    string query = "EXEC mantenclientes @op = 'loginadmin', @id = @id, @nombre = '', @cel = NULL, @correo = '', @direccion = '', @clave = @clave, @token = NULL, @tipo = '', @estado = '', @pregunta = '', @respuesta = '', @ip = @ip, @buscador = @buscador";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@clave", clave);
                        command.Parameters.AddWithValue("@ip", Request.UserHostAddress);
                        command.Parameters.AddWithValue("@buscador", Request.Browser.Browser);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return View("Tokenadmin");
                        }
                        if (contador <= 3)
                        {
                            var modelo = new MensajeCombinedViewModel
                            {
                                Mensaje = "Su clave es incorrecta"
                            };
                            contador = contador + 1;
                            return View("mensaje", modelo);
                        }
                        else
                        {
                            var modelo = new MensajeCombinedViewModel
                            {
                                Mensaje = "Su clave es incorrecta, su cuenta ha sido bloqueda, le invitamos a pasar por el apartado de recuperacion"
                            };
                            contador = contador + 1;
                            return View("mensaje", modelo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HttpStatusCodeResult(500, "Error en la base de datos");
            }
        }

        [HttpPost]
        public ActionResult Ingresoadmin(string id, string token)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "EXEC mantenclientes @op = 'token', @id = @id, @nombre = '', @cel = NULL, @correo = '', @direccion = '', @clave = '', @token = @token, @tipo = '', @estado = '', @pregunta = '', @respuesta = '', @ip = @ip, @buscador = @buscador";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@token", token);
                        command.Parameters.AddWithValue("@ip", Request.UserHostAddress);
                        command.Parameters.AddWithValue("@buscador", Request.Browser.Browser);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return View("Menuadmin");
                        }
                        else
                        {
                            var modelo = new ClaveInvalidadViewModel
                            {
                                Mensaje = "Toquen invalido, intentelo de nuevo"
                            };
                            return View("ClaveInvalidad", modelo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HttpStatusCodeResult(500, "Error en la base de datos");
            }
        }

/// //////////////////////////-------registro--------------///////////////////////////////////////////////////////////////
        public ActionResult Registro()
        {
            return View();
        }
        [HttpGet]
        //traer el api token
        private async Task<string> ObtenerTokenDesdeApiAsync()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var apiUrl = "http://127.0.0.1:5000/generate-token"; 


                    var response = await httpClient.GetStringAsync(apiUrl);


                    var tokenData = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(response);

                    return tokenData.Token;
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }
      
        public class TokenResponse
        {
            public string Token { get; set; }
        }

        public async Task<ActionResult> Agregarcliente(string id, string nombre, int? cel, string correo, string direccion, string clave, string pregunta, string respuesta)
        {
            if (!ValidarClave(clave))
            {
                var modelo = new MensajeCombinedViewModel
                {
                    Mensaje = "La clave debe tener entre 8 y 12 caracteres, incluir letras mayúsculas, minúsculas y al menos un número."
                };

                return View("mensaje", modelo);
            }

            var apiService = new ApiService();

            // Obtener token de la API
            string token = await apiService.ObtenerTokenDesdeApiAsync();

            if (string.IsNullOrEmpty(token))
            {
                var modelo = new MensajeCombinedViewModel
                {
                    Mensaje = "Hubo un error al generar el token. Intenta de nuevo más tarde."
                };

                return View("mensaje", modelo);
            }

            // Invertir las cadenas
            clave = ReverseString(clave);
            pregunta = ReverseString(pregunta);
            respuesta = ReverseString(respuesta);

            var cliente = new Cliente
            {
                Id = id,
                Nombre = nombre,
                Cel = cel,
                Correo = correo,
                Direccion = direccion,
                Clave = clave,
                Pregunta = pregunta,
                Respuesta = respuesta
            };

            try
            {
                var response = await apiService.VerificarRegistroAsync(cliente);

                if (response != null && response.Mensaje == "Se ha ingresado correctamente")
                {
                    var modelo = new MensajeCombinedViewModel
                    {
                        Mensaje = "Has sido registrado, tu token de ingreso es: " + response.Token + ", por favor ingresa por el login.",
                        Token = response.Token
                    };

                    return View("mensaje", modelo);
                }
                else
                {
                    var modelo = new MensajeCombinedViewModel
                    {
                        Mensaje = response?.Mensaje ?? "Hubo un error, vuelve a intentar más tarde"
                    };

                    return View("mensaje", modelo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var modelo = new MensajeCombinedViewModel
                {
                    Mensaje = "Hubo un error, vuelve a intentar más tarde"
                };

                return View("mensaje", modelo);
            }
        }


        /// /////////////////////carrito reposteria///////////////////////////

        public ActionResult FormCarritorepos()
        {
            return View();
        }
        
        
        private readonly ApiService _apiService = new ApiService();
        [HttpPost]
        public async Task<ActionResult> AddCarritorepos(string idcliente, string productos, int cantidad)
        {
            try
            {
                var modelo = await _apiService.AddCarritoreposAsync(idcliente, productos, cantidad);
                return View("BoletaConfir", modelo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var modelo = new MensajeViewModel
                {
                    Mensaje = "Hubo un error, vuelve a intentar más tarde"
                };
                return View("mensaje", modelo);
            }
        }

        /// ///////////////////////////     carrito -historial  reposteria ///////////////////////////////////////////
        public ActionResult Consultacarrito()
        {
            return View();
        }
        public ActionResult Consultahisrepos()
        {
            return View();
        }
        //historial
        public ActionResult Consultarahistorialrepos(string idcliente)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "EXEC mantenrepos @op = 'consultahis', @idpedido = '', @idcliente = @idcliente, @productos = '', @cantidad = '', @precio='', @estado='', @ip=@ip, @buscador=@buscador;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idcliente", idcliente);
                        command.Parameters.AddWithValue("@ip", Request.UserHostAddress);
                        command.Parameters.AddWithValue("@buscador", Request.Browser.Browser);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                ViewBag.IdPedido = reader["idpedido"].ToString();
                                ViewBag.IdCliente = reader["idcliente"].ToString();
                                ViewBag.Productos = reader["productos"].ToString();
                                ViewBag.Cantidad = reader["cantidad"].ToString();
                                ViewBag.Precio = reader["precio"].ToString();
                                ViewBag.Total = reader["total"].ToString();

                                return View("Historial");
                            }
                            else
                            {
                                var modelo = new MensajeCombinedViewModel
                                {
                                    Mensaje = "Hubo un error, vuelve a intentar más tarde"
                                };
                                return View("mensaje", modelo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var modelo = new MensajeCombinedViewModel
                {
                    Mensaje = "Hubo un error, vuelve a intentar más tarde"
                };
                return View("mensaje", modelo);
            }

        }
        //carrito
        public ActionResult Consultarcarritorepos(string idcliente)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            List<Carritorepos> pedidos = new List<Carritorepos>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "EXEC mantenrepos @op = 'consultacarrito', @idpedido = '', @idcliente = @idcliente, @productos = '', @cantidad = '', @precio='', @estado='', @ip=@ip, @buscador=@buscador;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idcliente", idcliente);
                        command.Parameters.AddWithValue("@ip", Request.UserHostAddress);
                        command.Parameters.AddWithValue("@buscador", Request.Browser.Browser);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Carritorepos pedido = new Carritorepos
                                {
                                    IdPedido = reader["idpedido"].ToString(),
                                    IdCliente = reader["idcliente"].ToString(),
                                    Productos = reader["productos"].ToString(),
                                    Cantidad = reader["cantidad"].ToString(),
                                    Precio = reader["precio"].ToString(),
                                    Total = reader["total"].ToString(),
                                    Estado = reader["estado"].ToString()
                                };
                                pedidos.Add(pedido);
                            }
                        }
                    }
                }

                return View("Carrito_r", pedidos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var modelo = new MensajeCombinedViewModel
                {
                    Mensaje = "Hubo un error, vuelve a intentar más tarde"
                };
                return View("mensaje", modelo);
            }

        }
        public ActionResult Consultapagar()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Pagar_r(string numeroTarjeta, decimal? monto, string idcliente)
        {
            if (string.IsNullOrEmpty(numeroTarjeta) || monto == null || monto <= 0 || string.IsNullOrEmpty(idcliente))
            {
                var modelo = new MensajeCombinedViewModel
                {
                    Mensaje = "Número de tarjeta, monto e idcliente son requeridos y el monto debe ser mayor a cero"
                };
                return View("mensaje", modelo);
            }

            var apiUrl = "http://127.0.0.1:5000/pagar";

            var requestData = new
            {
                numero_tarjeta = numeroTarjeta,
                monto,
                idcliente
            };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseString);

                    ViewBag.Mensaje = result.message;
                    return View("Mensaje", result);
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return new HttpStatusCodeResult((int)response.StatusCode, responseString);
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, $"Error al llamar al servicio: {ex.Message}");
            }
        }
        //////////////////////////queque personalizado /////////////////////////
        public ActionResult Encargo()
        {
            return View();
        }

        public async Task<ActionResult> Confirencargo(string idcliente, string sabor, string porciones, string cobertura, string colorcobertura, string fecha, string hora, string domicilio, string direccion)
        {
            try
            {
                var mensaje = await _apiService.ConfirEncargoAsync(idcliente, sabor, porciones, cobertura, colorcobertura, fecha, hora, domicilio, direccion);
                var modelo = new MensajeCombinedViewModel
                {
                    Mensaje = mensaje
                };
                return View("mensaje", modelo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var modelo = new MensajeCombinedViewModel
                {
                    Mensaje = "Hubo un error, vuelve a intentar más tarde"
                };
                return View("mensaje", modelo);
            }
        }

        ///////////////////////caterin/////////////////////////
        public ActionResult Formcaterin()
        {
            return View();
        }

        public async Task<ActionResult> Confircaterin(string idcliente, string degustacion, string fech_hora_degus, string direc, string fechaevento, string hora, string paquete, string precio)
        {
            try
            {
                var mensaje = await _apiService.ConfirCateAsync(idcliente, degustacion, fech_hora_degus, direc, fechaevento, hora, paquete, precio);
                var modelo = new MensajeCombinedViewModel
                {
                    Mensaje = mensaje
                };
                return View("mensaje", modelo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var modelo = new MensajeCombinedViewModel
                {
                    Mensaje = "Hubo un error, vuelve a intentar más tarde"
                };
                return View("mensaje", modelo);
            }
        }


        /// ///////////////////////////carrito -historial  caterin///////////////////////////////////////////
        public ActionResult Consultacaterin()
        {
            return View();
        }
        public ActionResult Consultarahistorialcate(string idcliente)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            List<Caterin> caterins = new List<Caterin>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "EXEC mantencaterin @op = 'consulta', @idcate = '', @idcliente = @idcliente, @degustacion= '', @fech_hora_degus= '', @direc= '', @fechaevento='', @hora='', @paquete = '', @precio = '', @ip= @ip, @buscador= @buscador;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idcliente", idcliente);
                        command.Parameters.AddWithValue("@ip", Request.UserHostAddress);
                        command.Parameters.AddWithValue("@buscador", Request.Browser.Browser);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Caterin caterin = new Caterin
                                {
                                    Idcate = reader["idcate"].ToString(),
                                    IdCliente = reader["idcliente"].ToString(),
                                    Paquete = reader["paquete"].ToString(),
                                    Precio = reader["precio"].ToString()

                                };
                                caterins.Add(caterin);
                            }
                            
                        }
                    }
                }
                return View("HistorialCate", caterins);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var modelo = new MensajeCombinedViewModel
                {
                    Mensaje = "Hubo un error, vuelve a intentar más tarde"
                };
                return View("mensaje", modelo);
            }

        }

        /// ///////////----------admin-----------------/////////////

        public ActionResult VerAuditorias()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            List<AuditoriaViewModel> auditorias = new List<AuditoriaViewModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "EXEC mantenclientes @op='auditoria', @id = '', @nombre = '', @cel = '', @correo='', @direccion='', @clave='', @token='', @tipo='', @estado= '',@pregunta='', @respuesta='', @ip= '', @buscador=''";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AuditoriaViewModel auditoria = new AuditoriaViewModel
                                {
                                    Idcliente = reader["idcliente"].ToString(),
                                    FechaHoraLogin = Convert.ToDateTime(reader["fecha_hora_login"]),
                                    Ip = reader["ip"].ToString(),
                                    Buscador = reader["buscador"].ToString(),
                                    BloqueosClave = reader["bloqueos_clave"] as int?,
                                    Accion = reader["accion"].ToString()
                                };
                                auditorias.Add(auditoria);
                            }
                        }
                    }
                }

                return View("Auditoria", auditorias);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var modelo = new MensajeCombinedViewModel
                {
                    Mensaje = "Hubo un error, vuelve a intentar más tarde"
                };
                return View("mensaje", modelo);
            }
        }


        [HttpGet]
        public ActionResult CambiarPrecios()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            List<Productos> productos = new List<Productos>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "EXEC mantenproductos @op='consultar', @nombre='', @precio=''";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Productos producto = new Productos
                                {
                                    Idproducto = reader["idproducto"].ToString(),
                                    Nombre = reader["nombre"].ToString(),
                                    Precio = reader["precio"].ToString(),
                                };
                                productos.Add(producto);
                            }
                        }
                    }
                }

                return View("Productos", productos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var modelo = new MensajeCombinedViewModel
                {
                    Mensaje = "Hubo un error, vuelve a intentar más tarde"
                };
                return View("mensaje", modelo);
            }
        }

        /// cambiar precio
        public ActionResult ActualizarPrecio(string precio, string nombre)
        {
            // Llamar al método asíncrono para consumir la API
            var result = ActualizarPrecioEnApiAsync(precio, nombre).Result;

            // Convertir la respuesta en el modelo adecuado
            var modelo = new MensajeCombinedViewModel
            {
                Mensaje = result
            };

            return View("mensaje", modelo);
        }
        private async Task<string> ActualizarPrecioEnApiAsync(string precio, string nombre)
        {
            var url = "http://localhost:5000/actualizar_precio"; // URL de la API en Python

            var content = new
            {
                nombre,
                precio
            };

            var jsonContent = JsonConvert.SerializeObject(content);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return responseString;
                }
                else
                {
                    return "Error en la solicitud: " + response.ReasonPhrase;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Hubo un error, vuelve a intentar más tarde";
            }
        }

        //----------------validar clave-----------------//
        private bool ValidarClave(string clave)
        {
            // - Tiene entre 8 y 12 caracteres
            // - Contiene al menos una letra mayúscula
            // - Contiene al menos una letra minúscula
            // - Contiene al menos un número
            string patron = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,12}$";
            return Regex.IsMatch(clave, patron);
        }

        
        //////----------------bloqueo-------------------//
        public ActionResult Consultadesbloqueo()
        {
            return View();
        }

        public ActionResult Formdesbloqueo(string idcliente, string pregunta, string respuesta)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    
                    string query = "EXEC mantenclientes @op='preguntas', @id = @id, @nombre = '', @cel = '', @correo='', @direccion='', @clave='', @token='', @tipo='', @estado= '', @pregunta='', @respuesta='', @ip= @ip, @buscador=@buscador;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", idcliente);
                        command.Parameters.AddWithValue("@ip", Request.UserHostAddress);
                        command.Parameters.AddWithValue("@buscador", Request.Browser.Browser);


                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                ViewBag.IdCliente = idcliente; 
                                ViewBag.Pregunta = ReverseString(reader["pregunta"].ToString());
                                ViewBag.Respuesta = ReverseString(reader["respuesta"].ToString());
                                command.Parameters.AddWithValue("@ip", Request.UserHostAddress);
                                command.Parameters.AddWithValue("@buscador", Request.Browser.Browser);


                                return View("Formdesbloqueo");
                            }
                            else
                            {
                                var modelo = new MensajeCombinedViewModel
                                {
                                    Mensaje = "Hubo un error, vuelve a intentar más tarde"
                                };
                                return View("mensaje", modelo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var modelo = new MensajeCombinedViewModel
                {
                    Mensaje = "Hubo un error, vuelve a intentar más tarde"
                };
                return View("mensaje", modelo);
            }
        }

        public async Task<ActionResult> Desbloqueo(string idcliente, string pregunta, string respuesta)
        {
            pregunta = ReverseString(pregunta);
            respuesta = ReverseString(respuesta);

            string token = await ObtenerTokenDesdeApiAsync();
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "EXEC mantenclientes @op='desbloquear', @id = @idcliente, @nombre = '', @cel = '', @correo='', @direccion='', @clave='', @token=@token, @tipo='', @estado= '',@pregunta=@pregunta, @respuesta=@respuesta, @ip= @ip, @buscador=@buscador";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idcliente", idcliente);
                        command.Parameters.AddWithValue("@token", token);
                        command.Parameters.AddWithValue("@pregunta", pregunta);
                        command.Parameters.AddWithValue("@respuesta", respuesta);
                        command.Parameters.AddWithValue("@ip", Request.UserHostAddress);
                        command.Parameters.AddWithValue("@buscador", Request.Browser.Browser);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                                var modelo = new MensajeCombinedViewModel
                                {
                                    Mensaje = "Su cuenta ha sido desbloqueada, por su seguridad su nuevo token de ingreso es: " + token + ", por favor ingrese por el login.",
                                    Token = token
                                };
                                return View("mensaje", modelo);
                            }
                            else
                            {
                                var modelo = new MensajeCombinedViewModel
                                {
                                    Mensaje = "Hubo un error, vuelve a intentar más tarde"
                                };
                                return View("mensaje", modelo);
                            }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var modelo = new MensajeCombinedViewModel
                {
                    Mensaje = "Hubo un error, vuelve a intentar más tarde"
                };
                return View("mensaje", modelo);
            }
        }

        //mini cifrado 
        public static string ReverseString(string input)
        {
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        // Método para obtener países
        public JsonResult GetPaises()
        {
            var paises = new List<dynamic>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand("consulta_paises", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        paises.Add(new { id = reader["pais_id"], detalle = reader["pais"] });
                    }
                }

                return Json(paises, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var error = new { mensaje = "Error al obtener los países", detalle = ex.Message };
                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }

        // Método para obtener provincias
        public JsonResult GetProvincias(int idpais)
        {
            var provincias = new List<dynamic>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("consulta_provincias", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@idpais", idpais);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    provincias.Add(new { id = reader["provincia_id"], detalle = reader["provincia"] });
                }
            }

            return Json(provincias, JsonRequestBehavior.AllowGet);
        }

        // Método para obtener cantones
        public JsonResult GetCantones(int idprovincia)
        {
            var cantones = new List<dynamic>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("consulta_cantones", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@idprovincia", idprovincia);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    cantones.Add(new { id = reader["canton_id"], detalle = reader["canton"] });
                }
            }

            return Json(cantones, JsonRequestBehavior.AllowGet);
        }

        // Método para obtener distritos
        public JsonResult GetDistritos(int idcanton)
        {
            var distritos = new List<dynamic>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("consulta_distritos", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@idcanton", idcanton);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    distritos.Add(new { id = reader["distrito_id"], detalle = reader["distrito"] });
                }
            }

            return Json(distritos, JsonRequestBehavior.AllowGet);
        }
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //---------------------------------------------------------------
    }
}
