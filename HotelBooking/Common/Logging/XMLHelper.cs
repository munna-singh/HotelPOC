using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TE.Common.Logging;

namespace Common.Logging
{
    public static class XMLHelper
    {
        public static string FormatXml(string sUnformattedXml)
        {
            Logger.Instance.Info("AmadeusMessagingHelper", "FormatXml", "BEGIN >>>");

            //will hold formatted xml
            StringBuilder sb = new StringBuilder();

            //make sure there are no invalid characters in the string, i.e, < >, &
            sUnformattedXml = sUnformattedXml.Replace("&", "and");
            //does the formatting
            XmlTextWriter xtw = null;
            try
            {
                //load unformatted xml into a dom
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(sUnformattedXml);

                //pumps the formatted xml into the StringBuilder above
                StringWriter sw = new StringWriter(sb);

                //point the xtw at the StringWriter
                xtw = new XmlTextWriter(sw);

                //we want the output formatted
                xtw.Formatting = Formatting.Indented;

                //get the dom to dump its contents into the xtw 
                xd.WriteTo(xtw);
            }
            catch (Exception ex)
            {
                Logger.Instance.Info("AmadeusMessagingHelper", "FormatXml", ex);
                Logger.Instance.Info("AmadeusMessagingHelper", "FormatXml", "END ^^^");
                return sUnformattedXml;
            }
            finally
            {
                //clean up even if error
                if (xtw != null)
                {
                    xtw.Close();
                }
            }

            Logger.Instance.Info("AmadeusMessagingHelper", "FormatXml", "END ^^^");
            //return the formatted xml
            return sb.ToString();
        }
    }
}
