using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cabspot.Models;
using System.Net;
using System.Text;
using System.IO;

namespace Cabspot.Controllers.Clases
{
    public class Push
    {
        public string varMessage;
        private CabspotDB db = new CabspotDB();

        public Push(string msj)
        {
            this.varMessage = msj;
        }

        public void EnviarTodosTaxistas()
        {
            //todo
            //taxistas taxista = db.taxistas.Find();
            var t = from x in db.taxistas
                    select new { x.idTaxista, x.codigoTaxista, x.apikey };
            foreach (var a in t)
            {
                Enviar(a.apikey);
            }

        }
        public void EnviarTodosClientes()
        {
            //clientes Clientes = db.clientes.Find();
            var t = from x in db.clientes
                    select new { x.apikey, x.idCliente };
            foreach (var a in t)
            {
                Enviar(a.apikey);
            }

        }
        public void EnviarClientes(int Id)
        {
            //clientes Clientes = db.clientes.Find(Id);


            var t = from x in db.clientes
                    where x.idCliente == Id
                    select new { x.apikey };

            Enviar(t.First().apikey);

        }
        public void EnviarTAxista(int Id)
        {

            //taxistas taxista = db.taxistas.Find(Id);
            var t = from x in db.taxistas
                    where x.idTaxista == Id
                    select new { x.apikey };

            Enviar(t.First().apikey);

        }
        public void Enviar(string regId)
        {
            //string regId = "APA91bGVBkmBxCjP0BTifMdT9QWtQC9hv3DC9_dVLJIq9LMh5BT8g8IjDwdy7OStvpv5KaIGXx50offlE67DzjCDPH-6_kAznRVxxQc0jszalKWkcdPK3ABOpmz1Vdnlten8ddp3fKFe";

            //API Key created in Google project  
            //se ve feo aqui, seria bueno guardarlo en otro lado 
            var applicationID = "AIzaSyD85BpzskNxG5sg8udN3BeRezDj7mTVA9s";
            //Project ID created in Google project. 
            //esto tambien !!! 
            var SENDER_ID = "1094056874126";

            WebRequest tRequest;
            tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

            string postDataToServer = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.m="
                               + varMessage + "&message_id=" + 123 + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + regId + "";

            Byte[] byteArray = Encoding.UTF8.GetBytes(postDataToServer);
            tRequest.ContentLength = byteArray.Length;

            Stream dataStream = tRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse tResponse = tRequest.GetResponse();

            dataStream = tResponse.GetResponseStream();
                
            StreamReader tReader = new StreamReader(dataStream);

            String sResponseFromServer = tReader.ReadToEnd();

            tReader.Close();
            dataStream.Close();
            tResponse.Close();
        }
    }
}