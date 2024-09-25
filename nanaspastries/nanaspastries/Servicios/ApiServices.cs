using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using nanaspastries.Controllers;
using Newtonsoft.Json;

public class Cliente
{
    public string Id { get; set; }
    public string Nombre { get; set; }
    public int? Cel { get; set; }
    public string Correo { get; set; }
    public string Direccion { get; set; }
    public string Clave { get; set; }
    public string Pregunta { get; set; }
    public string Respuesta { get; set; }
}

public class MensajeResponse
{
    public string Mensaje { get; set; }
    public string Token { get; set; }
}

public class ApiService
{
    private static readonly HttpClient client = new HttpClient();

    //registro
    public async Task<MensajeResponse> VerificarRegistroAsync(Cliente cliente)
    {
        var url = "http://127.0.0.1:5000/verificar_registro";
        var json = JsonConvert.SerializeObject(cliente);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, data);
        var result = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<MensajeResponse>(result);
    }
    //token
    public async Task<string> ObtenerTokenDesdeApiAsync()
    {
        var url = "http://127.0.0.1:5000/generate-token";
        var response = await client.GetAsync(url);
        var result = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, int>>(result);
        return tokenResponse["token"].ToString();
    }
    //carrito reposteria
    public async Task<BoletaViewModel> AddCarritoreposAsync(string idcliente, string productos, int cantidad)
    {
        var url = "http://127.0.0.1:5000/add-carritorepos";
        var requestData = new
        {
            idcliente,
            productos,
            cantidad
        };

        var json = JsonConvert.SerializeObject(requestData);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, data);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var boleta = JsonConvert.DeserializeObject<BoletaViewModel>(result);
        return boleta;
    }
    //encargo personalizado
    public async Task<string> ConfirEncargoAsync(string idcliente, string sabor, string porciones, string cobertura, string colorcobertura, string fecha, string hora, string domicilio, string direccion)
    {
        var url = "http://127.0.0.1:5000/confir-encargo";
        var requestData = new
        {
            idcliente,
            sabor,
            porciones,
            cobertura,
            colorcobertura,
            fecha,
            hora,
            domicilio,
            direccion,
            ip = HttpContext.Current.Request.UserHostAddress,
            buscador = HttpContext.Current.Request.Browser.Browser
        };

        var json = JsonConvert.SerializeObject(requestData);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, data);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"El código de estado de la respuesta no indica un resultado correcto: {(int)response.StatusCode} ({response.ReasonPhrase}).");
        }


        var result = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
        return responseObject["message"];
    }

    //caterin 
    public async Task<string> ConfirCateAsync(string idcliente, string degustacion, string fech_hora_degus, string direc, string fechaevento, string hora, string paquete, string precio)
    {
        var url = "http://127.0.0.1:5000/add-carritocate";
        var requestData = new
        {
            idcliente,
            degustacion,
            fech_hora_degus,
            direc,
            fechaevento,
            hora,
            paquete,
            precio,
            ip = HttpContext.Current.Request.UserHostAddress,
            buscador = HttpContext.Current.Request.Browser.Browser
        };

        var json = JsonConvert.SerializeObject(requestData);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, data);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"El código de estado de la respuesta no indica un resultado correcto: {(int)response.StatusCode} ({response.ReasonPhrase}).");
        }

        var result = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
        return responseObject["message"];
    }
    //tipo de cambio 
    public async Task<double> ObtenerTipoCambioAsync()
    {
        var url = "http://127.0.0.1:5000/tipo-cambio"; // URL del endpoint Flask

        HttpResponseMessage response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"El código de estado de la respuesta no indica un resultado correcto: {(int)response.StatusCode} ({response.ReasonPhrase}).");
        }

        string content = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<Dictionary<string, double>>(content);

        return data["tipo_cambio"];
    }

}


