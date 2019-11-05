using System.Collections.Generic;
using System.Xml.Serialization;

namespace SmartwallPackage.Language
{
    /// <summary>
    /// Container class for the language data of a language option.
    /// This class is for loading in the XML files.
    /// NOTE the files have to be named in acordance with ISO 639-1 standard for langauge codes.
    /// </summary>
    public class LanguageContainer
    {
        //all text entries
        [XmlArray("TextEntries")]
        [XmlArrayItem("Text")]
        public List<string> Texts;

        //Paths to images
        [XmlArrayItem("Path")]
        public List<string> Images;

        //paths to audio files, Files have to be a Wav format
        [XmlArrayItem("Path")]
        public List<string> AudioFiles;

        public LanguageContainer()
        {
            Texts      = new List<string>();
            Images     = new List<string>();
            AudioFiles = new List<string>();
        }
    }
}
