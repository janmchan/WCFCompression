
using System;
using System.IO;
using System.IO.Compression;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.ServiceModel.Web;

namespace WcfExtensions
{
    public class GZipMessageEncoder : MessageEncoder
    {


        static string GZipContentType = @"application/soap+xml";

        //This implementation wraps an inner encoder that actually converts a WCF Message
        //into textual XML, binary XML or some other format. This implementation then compresses the results.
        //The opposite happens when reading messages.
        //This member stores this inner encoder.
        MessageEncoder innerEncoder;


        //We require an inner encoder to be supplied (see comment above)
        public GZipMessageEncoder(MessageEncoder messageEncoder)
            : base()
        {
            if (messageEncoder == null)
                throw new ArgumentNullException("messageEncoder", "A valid message encoder must be passed to the GZipEncoder");
            innerEncoder = messageEncoder;
        }



        public override string ContentType
        {
            get { return GZipContentType; }
        }

        public override string MediaType
        {
            get { return GZipContentType; }
        }

        //SOAP version to use - we delegate to the inner encoder for this
        public override MessageVersion MessageVersion
        {
            get { return innerEncoder.MessageVersion; }
        }


        static byte[] RegularBuffer(ArraySegment<byte> buffer, BufferManager bufferManager)
        {
            var stream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count - buffer.Offset);
            return stream.ToArray();
        }
        static ArraySegment<byte> DecompressBuffer(ArraySegment<byte> buffer, BufferManager bufferManager)
        {
            using (var memoryStream = new MemoryStream(buffer.Array, 0, buffer.Count))
            using (var decompressedStream = new MemoryStream())
            {
                var totalRead = 0;
                var blockSize = 1024;
                var tempBuffer = bufferManager.TakeBuffer(blockSize);
                using (var gzStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    while (true)
                    {
                        var bytesRead = gzStream.Read(tempBuffer, 0, blockSize);
                        if (bytesRead == 0)
                            break;
                        decompressedStream.Write(tempBuffer, 0, bytesRead);
                        totalRead += bytesRead;
                    }
                }
                bufferManager.ReturnBuffer(tempBuffer);

                byte[] decompressedBytes = decompressedStream.ToArray();
                //return decompressedBytes;
                byte[] bufferManagerBuffer = bufferManager.TakeBuffer(decompressedBytes.Length + buffer.Offset);
                Array.Copy(buffer.Array, 0, bufferManagerBuffer, 0, buffer.Offset);
                Array.Copy(decompressedBytes, 0, bufferManagerBuffer, buffer.Offset, decompressedBytes.Length);
                ArraySegment<byte> byteArray = new ArraySegment<byte>(bufferManagerBuffer, buffer.Offset, decompressedBytes.Length);
                bufferManager.ReturnBuffer(buffer.Array);

                return byteArray;
            }

        }


        //One of the two main entry points into the encoder. Called by WCF to decode a buffered byte array into a Message.
        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            /*var decompBytes = DecompressBuffer(buffer, bufferManager);
            var body = GetContentBody(decompBytes);
            var message = CreateMessageFromXml(body);
            message.Properties.Encoder = this;
            return message;*/
            try
            {
                buffer = DecompressBuffer(buffer, bufferManager);
            
            }
            catch (Exception)
            {
                //TODO: check header
            }
            var message = innerEncoder.ReadMessage(buffer, bufferManager, contentType);
            message.Properties.Encoder = this;
            message.Headers.To = new Uri("http://localhost:19860/Service1.svc");
            message.Headers.Action = @"http://tempuri.org/IService1/GetData";
            return message;
        }
        private string GetContentBody(byte[] request)
        {
            // Read the request content and then reset the stream position back to its original place.
            var contentStr = Encoding.UTF8.GetString(request).Trim();
            return contentStr;
        }
        private Message CreateMessageFromXml(string newXml)
        {
            var textReader = new StringReader(newXml);
            var xmlReader = XmlReader.Create(textReader, new XmlReaderSettings() { IgnoreWhitespace = false });
            var dictReader = XmlDictionaryReader.CreateDictionaryReader(xmlReader);
            return Message.CreateMessage(dictReader, int.MaxValue, this.MessageVersion);
        }
        //One of the two main entry points into the encoder. Called by WCF to encode a Message into a buffered byte array.
        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            //No need to compress response this will be done by IIS
            return innerEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
        }

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            var gzStream = new GZipStream(stream, CompressionMode.Decompress, true);
            return innerEncoder.ReadMessage(gzStream, maxSizeOfHeaders);
        }

        public override void WriteMessage(Message message, System.IO.Stream stream)
        {
            using (var gzStream = new GZipStream(stream, CompressionMode.Compress, true))
            {
                innerEncoder.WriteMessage(message, gzStream);
            }

            // innerEncoder.WriteMessage(message, gzStream) depends on that it can flush data by flushing 
            // the stream passed in, but the implementation of GZipStream.Flush will not flush underlying
            // stream, so we need to flush here.
            stream.Flush();
        }

    }
}

// end of namespace
