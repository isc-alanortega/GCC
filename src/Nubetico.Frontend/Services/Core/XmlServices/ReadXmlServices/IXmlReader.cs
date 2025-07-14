using System.Xml.Linq;

namespace Nubetico.Frontend.Services.Core.XmlServices.ReadXmlServices
{
    // <summary>
    /// The IXmlReader interface defines the contract for reading XML data 
    /// from a stream. Implementations of this interface must provide a 
    /// method to asynchronously read and parse XML data into an XDocument.
    /// </summary>
    public interface IXmlReader
    {
        /// <summary>
        /// Asynchronously reads XML data from the specified stream and returns 
        /// an XDocument containing the parsed XML.
        /// </summary>
        /// <param name="fileStream">The stream from which the XML data will be read.</param>
        /// <returns>A Task that represents the asynchronous operation, with a value of type XDocument 
        /// containing the parsed XML data.</returns>
        /// <exception cref="XmlException">Thrown when the XML is not well-formed.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs while reading from the stream.</exception>
        Task<XDocument> ReadXmlAsync(Stream fileStream);
    }
}
