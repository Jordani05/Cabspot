using Cabspot.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cabspot.Models;

namespace Cabspot.Controllers.Clases
{
    public class Utilidades
    {
        public static CabspotDB db = new CabspotDB();

        public static string RutaRelativa(string filePath, string referencePath)
        {
            var fileUri = new Uri(filePath);
            var referenceUri = new Uri(referencePath);
            return referenceUri.MakeRelativeUri(fileUri).ToString();
        }


        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static int getDistance(double oLat, double oLng, double dLat, double dLng)
        {
            int distance = 0;
            string api_key = "AIzaSyAjZNtyj1l5imtnp1v_M_aFCJFKVG_yPdQ";

            string url = String.Format("http://maps.googleapis.com/maps/api/distancematrix/json?origins={0},{1}&destinations={2},{3}&mode=driving&sensor=false&language=es-ES", oLat, oLng, dLat, dLng);
            string requestUrl = url;
            string content = fileGetContents(requestUrl);
            JObject o = JObject.Parse(content);

            try
            {
                distance = (int)o.SelectToken("rows[0].elements[0].distance.value");
                return distance / 1000;  //distancia en kilometros
            }
            catch (Exception e)
            {
                return distance;
            }
        }

        public static int getTravelTime(double oLat, double oLng, double dLat, double dLng)
        {
            int duration = 0;

            string url = String.Format("http://maps.googleapis.com/maps/api/distancematrix/json?origins={0},{1}&destinations={2},{3}&mode=driving&sensor=false&language=es-ES", oLat, oLng, dLat, dLng);
            string requestUrl = url;
            string content = fileGetContents(requestUrl);
            JObject o = JObject.Parse(content);

            try
            {
                duration = (int)o.SelectToken("rows[0].elements[0].duration.value");
                return duration / 60;  //duracion en minutos
            }
            catch (Exception e)
            {
                return duration;
            }
        }


        public static string getAddress(double lat, double lng)
        {
            string address = "";
            string api_key = "AIzaSyAjZNtyj1l5imtnp1v_M_aFCJFKVG_yPdQ";
            
            string url = String.Format("http://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&language=es-ES", lat, lng);
            string returnUrl = url;
            string content = Utilidades.fileGetContents(returnUrl);

            JObject o = JObject.Parse(content);

            try
            {
                address = (string)o.SelectToken("results[0].formatted_address");
                return address;
            }
            catch (Exception e)
            {
                return address;
            }
        }

        public static string fileGetContents(string fileName)
        {
            string sContents = string.Empty;
            string me = string.Empty;
            try
            {
                if (fileName.ToLower().IndexOf("http:") > -1)
                {
                    System.Net.WebClient wc = new System.Net.WebClient();
                    byte[] response = wc.DownloadData(fileName);
                    sContents = System.Text.Encoding.ASCII.GetString(response);

                }
                else
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
                    sContents = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch { sContents = "unable to connect to server "; }
            return sContents;
        }

        

    }
}