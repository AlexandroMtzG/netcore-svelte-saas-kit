using System.Xml;

namespace NetcoreSaas.Application.Helpers
{
    public static class XmlHelper
    {
        public static string GetValue(XmlDocument doc, string elementName, string attributeName, XmlNamespaceManager nsmgr)
        {
            try
            {
                var elemList = elementName.Contains("cfdi:") ? 
                    doc.SelectNodes(elementName, nsmgr) : 
                    doc.GetElementsByTagName(elementName);
                if (elemList != null)
                    foreach (XmlNode node in elemList)
                    {
                        if (node.Attributes == null) continue;
                        foreach (XmlAttribute attribute in node.Attributes)
                        {
                            if (attribute.Name.ToLower() == attributeName.ToLower())
                                return attribute.Value;
                        }
                    }
            }
            catch
            {
                // ignored
            }

            return "";
        }
        public static string GetValue(XmlElement element, string v)
        {
            try
            {
                foreach (XmlAttribute item in element.Attributes)
                {
                    if (item.Name.ToLower() == v.ToLower())
                    {
                        var value = element.Attributes[item.Name]?.InnerText;
                        return value;
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
        
        public static string GetValue(XmlElement doc, string elementName, string attributeName, XmlNamespaceManager nsmgr)
        {
            try
            {
                var elemList = elementName.Contains("cfdi:") ? doc.SelectNodes(elementName, nsmgr) : doc.GetElementsByTagName(elementName);
                if (elemList != null)
                    foreach (XmlNode node in elemList)
                    {
                        if (node.Attributes != null)
                            foreach (XmlAttribute attribute in node.Attributes)
                            {
                                if (attribute.Name.ToLower() == attributeName.ToLower())
                                    return attribute.Value;
                            }
                    }
            }
            catch
            {
                // ignored
            }

            return "";
        }
    }
}
