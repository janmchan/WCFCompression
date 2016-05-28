using System.Collections.ObjectModel;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;
using System.Diagnostics;
using System;

namespace WCFExtensions
{
    public class GZipInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            //Try to count the content length here
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm;ss");
            HttpRequestMessageProperty requestMessage = request.Properties["httpRequest"] as HttpRequestMessageProperty;
            var content = GetXmlContent(ref request);
            Debug.WriteLine($"{ timestamp} - Inspector: { content }");
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            //Do nothing
        }

        private Message CreateMessageFromXml(string newXml, MessageVersion messageVersion)
        {
            var textReader = new StringReader(newXml);
            var xmlReader = XmlReader.Create(textReader, new XmlReaderSettings() { IgnoreWhitespace = false });
            var dictReader = XmlDictionaryReader.CreateDictionaryReader(xmlReader);
            return Message.CreateMessage(dictReader, int.MaxValue, messageVersion);
        }

        private string GetXmlContent(ref Message request)
        {
            var buffer = request.CreateBufferedCopy(int.MaxValue);
            request = buffer.CreateMessage();

            var messageCopy = buffer.CreateMessage();
            return messageCopy.ToString();
        }

    }
}