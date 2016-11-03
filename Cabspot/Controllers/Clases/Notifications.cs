using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.NotificationHubs;
using System.Text;
using System.IO;
using System.Net;

namespace Cabspot.Controllers.Clases
{
    public class Notifications
    {
        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; set; }

        private Notifications()
        {
            Hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://cabspotpushnotificationshub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=jzNxQVUrtz7A5TBfN9QDdJMT5F6HvnV1E/jddfZSVa0=",   //"<your hub's DefaultFullSharedAccessSignature>",
                                                                         "cabspotpushnotificationshub");   //  "<hub name>");
        }

        public static void generarNotificaciones()
        {
            string regId = "APA91bGVBkmBxCjP0BTifMdT9QWtQC9hv3DC9_dVLJIq9LMh5BT8g8IjDwdy7OStvpv5KaIGXx50offlE67DzjCDPH-6_kAznRVxxQc0jszalKWkcdPK3ABOpmz1Vdnlten8ddp3fKFe";

            //API Key created in Google project  
            var applicationID = "AIzaSyD85BpzskNxG5sg8udN3BeRezDj7mTVA9s";

            //Project ID created in Google project.  
            var SENDER_ID = "1094056874126";
            var varMessage = "Hello";
            WebRequest tRequest;
            tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

            string postDataToServer = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message="
                               + varMessage + "&message_id=" + 123 + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + regId + "";

            Console.WriteLine(postDataToServer);
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