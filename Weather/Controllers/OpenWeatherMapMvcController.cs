using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Weather.Models;

namespace Weather.Controllers
{
    public class OpenWeatherMapMvcController : Controller
    {
        private static string ApiKey = "f53b600213f56cf6284a15e540b85796"; //ключ для доступа к OpenWeatherMap
        // GET: OpenWeatherMapMvc
        [HttpGet]
        public ActionResult Index()
        {
            ResponseWeather responseWeather = GetWeatherByCiti("Kyiv"); // Информция о погоде в Киеве отображается по умолчанию.
                                                                        //можно было бы использовать определение города по IP юзера но это коректно работает только с развернутым сайтом а не при использовании локального сервер
            return View("~/Views/OpenWeatherMapMvc/Search.cshtml",responseWeather); //вызов представления и передача модели
            
        }

        [HttpPost]
        public ActionResult Index(string cities)
        {
            ResponseWeather rootObject = null;
            if (!string.IsNullOrEmpty(cities)) //проверяем есть ли символы в строке
            {
                rootObject = GetWeatherByCiti(cities);//ищем погоду по названию города
                if (rootObject != null)//проверяем получили ли мы данные о погоде в указанном нами городе
                {
                     return View("~/Views/OpenWeatherMapMvc/Search.cshtml", rootObject);//вызов представления и передача модели
                }
            }
                return Content("You did not specify a city or the specified city was not found!");//Error
            
        }

        private static ResponseWeather GetWeatherByCiti(string cities)
        {
            ResponseWeather rootObject;
            HttpWebRequest apiRequest = WebRequest.Create("https://api.openweathermap.org/data/2.5/weather?q="
                                        + cities + "&appid=" + ApiKey + "&units=metric") as HttpWebRequest;//запрос по названию города

            string apiResponse = "";
            try
            {
                using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)//используем using для подключению в к вyешнему Api
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());//создаём поток для чтения данных
                    apiResponse = reader.ReadToEnd();//считываем данные и получаем JSON
                }
            }
            catch (Exception)
            {
                return null;
            }
           
            rootObject = JsonConvert.DeserializeObject<ResponseWeather>(apiResponse);//преобразовываем получиный JSON в модель ResponseWeather
            return rootObject;
        }
    }
}