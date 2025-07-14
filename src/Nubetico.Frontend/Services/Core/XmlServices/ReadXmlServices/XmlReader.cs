using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace Nubetico.Frontend.Services.Core.XmlServices.ReadXmlServices
{
    // <summary>
    /// The XmlReader class is responsible for reading XML data from a given stream.
    /// It implements the IXmlReader interface, providing an asynchronous method to 
    /// read XML content and parse it into an XDocument object.
    /// </summary>
    public class XmlReader : IXmlReader
    {
        /// <summary>
        /// Asynchronously reads XML data from the specified stream and returns an XDocument.
        /// This method reads the stream in chunks and appends the content to a StringBuilder,
        /// which is then parsed into an XDocument.
        /// </summary>
        /// <param name="fileStream">The stream from which the XML data will be read.</param>
        /// <returns>A Task that represents the asynchronous operation, with a value of type XDocument 
        /// containing the parsed XML data.</returns>
        /// <exception cref="XmlException">Thrown when the XML is not well-formed.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs while reading from the stream.</exception>
        public async Task<XDocument> ReadXmlAsync(Stream fileStream)
        {
            using (var reader = new StreamReader(fileStream))
            {
                char[] buffer = new char[1024];
                StringBuilder sb = new StringBuilder();

                int bytesRead;
                while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    sb.Append(buffer, 0, bytesRead);
                }

                return XDocument.Parse(sb.ToString());
            }
        }
    }
}
