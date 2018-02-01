using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BattleShipNet.Helpers
{
    public static class MessageHandler {

        /// <summary>
        /// Print Bootstrap Alerts messages
        /// </summary>
        /// <param name="type">Bootstrap css-class extension (string)</param>
        /// <param name="messages">Messages to print (List of strings)</param>
        /// <returns>Html to print (HtmlString)</returns>
        public static HtmlString Alerts(Dictionary<string, List<string>> messages)
        {
            if (messages != null && messages.Count > 0)
            {
                string html = "";

                foreach (KeyValuePair<string, List<string>> pair in messages)
                {
                    html += Alert(pair.Key, pair.Value).ToString();
                }

                return new HtmlString(html);
            }

            return new HtmlString(null);
        }

        /// <summary>
        /// Print Bootstrap Alert message
        /// </summary>
        /// <param name="type">Bootstrap css-class extension (string)</param>
        /// <param name="messages">Messages to print (List of strings)</param>
        /// <returns>Html to print (HtmlString)</returns>
        public static HtmlString Alert(string type, List<string> messages)
        {
            if (messages != null && messages.Count > 0)
            {
                TagBuilder div = new TagBuilder("div");
                div.AddCssClass("alert alert-" + type);
                div.Attributes.Add("role", "alert");
                div.Attributes.Add("id", "alert-" + type);

                TagBuilder ul = new TagBuilder("ul");

                foreach (string message in messages)
                {
                    TagBuilder li = new TagBuilder("li");
                    li.InnerHtml = message;
                    ul.InnerHtml += li.ToString();
                }

                div.InnerHtml = ul.ToString();

                return new HtmlString(div.ToString());
            }

            return new HtmlString(null);
        }
    }
}