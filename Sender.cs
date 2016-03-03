using System;
using System.IO;
using System.Messaging;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
 
public class Sender
    {
        private readonly string _targetMsmqEndpoint;
 
        public Sender (string targetMsmqEndpoint)
        {
            _targetMsmqEndpoint = targetMsmqEndpoint;
        }
 
    
        public void Send<T>(T message)
        {
            var xml = SerializeWithNameSpace(message);
 
            var bytes = new UTF8Encoding().GetBytes(xml);
            var stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
 
            using (var messageQueue = new MessageQueue(GetEndpoint()))
            {
                messageQueue.Send(new Message() { BodyStream = stream }, MessageQueueTransactionType.Single);
            }
        }
 
        private string GetEndpoint()
        {
            try
            {
                var endpointArray = _targetMsmqEndpoint.Split('@');
                var queue = endpointArray[0];
                var host = endpointArray[1];
 
                if (host.Equals("localhost", StringComparison.InvariantCultureIgnoreCase))
                {
                    return string.Format(@".\Private$\{0}", queue);
                }
                return string.Format(@"formatname:DIRECT=OS:{1}\private$\{0}", queue, host);
            }
            catch (Exception)
            {
                throw new Exception("Invalid Msmq Endpoint");
            }
        }
 
        private static string SerializeWithNameSpace<T>(T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T), "http://tempuri.net/" + typeof(T).Namespace);
                var stringWriter = new Utf8StringWriter();
 
                using (var writer = XmlWriter.Create(stringWriter))
               {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }
 
        class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }
    }