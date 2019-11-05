#pragma warning disable 0168
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

/* READ ME
 * This class is a little overly complex, this is mostly because i wanted to support absolute and relative paths.
 * This allows you to load XML files without wurrieing about where the application is placed on the PC but it also 
 * doesn't stop you from using it on files outside the game folder. Most of the code below is simply checking if the 
 * parameters are valid.
 * 
 * If you wish to see just the saving or loading to/from XML look at the last few lines of "_LoadClassFromXML<T>" 
 * and "SaveClassToXML<T>". If the <T> is unknown to you check out "Generic methods" here: 
 * https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/generic-methods
 * Basicly it makes it so you can pass anything to this method.
 */

namespace SmartwallPackage.Utils
{
    /// <summary>
    /// This is a Utility class for loading and saving container classes to or from XML files.
    /// It will not stop you from useing it with non container classes and will work on any class.
    /// A List is also a class so those work as well.
    /// </summary>
    public static class XML_to_Class
    {

        /// <summary>
        /// Loads a class from an XML file at the given path.
        /// Path is relative to the game data folder.
        /// Returns the class or NULL of the xml file was not found.
        /// Throws an ArgumentException if the given path leads to a non xml file, the path if absolute, or the path is invalid.
        /// </summary>
        /// <typeparam name="T">Reference to what class is expected in the XML file.</typeparam>
        /// <param name="filePath">Path of the file relative to the game data folder.</param>
        /// <returns>Instance of the givven class loaded from the XML, NULL if the file is not found.</returns>
        public static T LoadClassFromXML<T>(string relativeFilePath)
        {
            try
            {
                if (Path.IsPathRooted(relativeFilePath))
                {
                    if (relativeFilePath.Contains(Path.VolumeSeparatorChar.ToString()))
                    {
                        throw new ArgumentException("XML_to_Class | LoadClassFromXML | Given path is an absolute path, this is not allowed as it mite not work on other machines. Use \"LoadClassFromXMLOutsideData\" if you must load using an absolute path.");
                    }
                    else
                    {
                        //remove rooting from the path otherwise we will end up with 2 seperator chars between the data path and the relative path.
                        relativeFilePath = relativeFilePath.Substring(1);
                    }
                }
                return _LoadClassFromXML<T>(relativeFilePath, true);
            }
            catch (ArgumentException aEx)
            {
                throw new ArgumentException("The given path is invalid: " + relativeFilePath);
            }
        }

        /// <summary>
        /// Loads a class from an XML file outside the applications data folder, at the given path.
        /// Path is absolute.
        /// Returns the class or NULL of the xml file was not found.
        /// Throws an ArgumentException if the given path leads to a non xml file, the path is not absolute, or the path is invalid.
        /// </summary>
        /// <typeparam name="T">Reference to what class is expected in the XML file.</typeparam>
        /// <returns>Instance of the givven class loaded from the XML, NULL if the file is not found.</returns>
        public static T LoadClassFromXMLOutsideData<T>(string absoluteFilePath)
        {
            bool temp;
            try
            {
                temp = !(Path.IsPathRooted(absoluteFilePath) && absoluteFilePath.Contains(Path.VolumeSeparatorChar.ToString()));
            }
            catch (ArgumentException aEx)
            {
                throw new ArgumentException("XML_to_Class | LoadClassFromXMLOutsideData | The given path is invalid: " + absoluteFilePath);
            }
            if (temp)
            {
                throw new ArgumentException("XML_to_Class | LoadClassFromXMLOutsideData | Given path is NOT an absolute path, this method is for reading XML files that are not in the aplications data folder, as sutch it doesn't know what this path is relative to.  Use \"LoadClassFromXML\" if you want to load an XML file in the applications data folder.");
            }
            return _LoadClassFromXML<T>(absoluteFilePath, false);
        }

        //Further handels checking of parameters and does the actual loading.
        private static T _LoadClassFromXML<T>(string FilePath, bool isRelative)
        {
            //Check given path for mistakes.
            if (FilePath.Equals(string.Empty))
            {
                throw new ArgumentException("XML_to_Class | LoadClassFromXML | Path is empty.");
            }
            string extention = Path.GetExtension(FilePath);
            if (extention == string.Empty) //User probably forgot to add .xml to the end of the path.
            {
                FilePath += ".xml";
            }
            else if (extention != ".xml")
            {
                throw new ArgumentException("XML_to_Class | LoadClassFromXML | The file is not an XML file.");
            }

            string absoluteFilePath;
            if (isRelative)
            {
                //Turn relative path into the absolute path
                absoluteFilePath = Application.dataPath + Path.DirectorySeparatorChar + FilePath;
            }
            else
            {
                //Path is already absolute
                absoluteFilePath = FilePath;
            }

            //If xml file does not exsist return null and post warning in console
            if (!File.Exists(absoluteFilePath)) { Debug.LogWarning("XML_to_Class | LoadClassFromXML(OutsideData) | Atempted to load non-exsistant XML file: " + absoluteFilePath); return default; }

            //Actual loading of the file
            XmlSerializer s      = XmlSerializer.FromTypes(new[] { typeof(T) })[0];
            FileStream    stream = new FileStream(absoluteFilePath, FileMode.Open, FileAccess.Read);
            T             temp;
            try
            {
                temp = (T)s.Deserialize(stream);
                stream.Dispose();
            }
            catch (Exception XEx)
            {
                Debug.LogError("XML file corrupted: " + Path.GetFileName(absoluteFilePath));
                stream.Dispose();
                File.Delete(absoluteFilePath);
                temp = default;
            }
            return temp;
        }


        //Saving

        /// <summary>
        /// Saves a class to an XML file in the given folder with the given filename.
        /// The path is relative to the games data folder.
        /// Will throw agument exceptions on invalid parameters.
        /// Optional parameter wether the file should be overwritten if it already exsists, default is true.
        /// returns true if the file was saved, false if it was not saved to prevent overwriting.
        /// </summary>
        /// <param name="classToSave">the instance of a class you wish to save to XML</param>
        /// <param name="folder">path of the folder relative to the data folder</param>
        /// <param name="fileName">name for the file</param>
        /// <param name="overWrite">If the file already exsists should it be overwritten</param>
        public static bool SaveClassToXML(object classToSave, string folder, string fileName, bool overWrite = true)
        {
            if (Path.IsPathRooted(folder))
            {
                if (folder.Contains(Path.VolumeSeparatorChar.ToString()))
                {
                    throw new ArgumentException("XML_to_Class | SaveClassToXML | Given path is an absolute path, this is not allowed as it mite not work on other machines. Use \"SaveClassToXMLOutsideData\" if you must save using an absolute path.");
                }
                else
                {
                    //remove rooting from the path otherwise we will end up with 2 seperator chars between the data path and the relative path.
                    folder = folder.Substring(1);
                }
            }
            return _SaveClassToXML(classToSave, folder, fileName, overWrite, true);
        }
        ///<summary>Overload for complete path</summary>
        public static bool SaveClassToXML(object classToSave, string relativePath, bool overWrite = true)
        {
            string filename = Path.GetFileName(relativePath);
            string folder   = Path.GetDirectoryName(relativePath);
            try
            {
                return SaveClassToXML(classToSave, folder, filename, overWrite);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Saves a class to an XML file in the given folder with the given filename.
        /// The path is the full path to the folder.
        /// Will throw agument exceptions on invalid parameters.
        /// Optional parameter wether the file should be overwritten if it already exsists, default is true.
        /// returns true if the file was saved, false if it was not saved to prevent overwriting.
        /// </summary>
        /// <param name="classToSave">the instance of a class you wish to save to XML</param>
        /// <param name="folder">path of the folder relative to the data folder</param>
        /// <param name="fileName">name for the file</param>
        /// <param name="overWrite">If the file already exsists should it be overwritten</param>
        public static bool SaveClassToXMLOutsideData(object classToSave, string folder, string fileName, bool overWrite = true)
        {
            try
            {
                if (!(Path.IsPathRooted(folder) && folder.Contains(Path.VolumeSeparatorChar.ToString())))
                {
                    throw new ArgumentException("XML_to_Class | SaveClassToXMLOutsideData | Given path is NOT an absolute path, this method is for saving into XML files that are not in the aplications data folder, as sutch it doesn't know what this path is relative to.  Use \"SaveClassFromXML\" if you want to save to an XML file in the applications data folder.");
                }
                return _SaveClassToXML(classToSave, folder, fileName, overWrite, false);
            }
            catch (ArgumentException aEx)
            {
                throw new ArgumentException("XML_to_Class | SaveClassToXMLOutsideData | The given path is invalid: " + folder);
            }
        }
        ///<summary>Overload for complete path.</summary>
        public static bool SaveClassToXMLOutsideData(object classToSave, string absolutePath, bool overWrite = true)
        {
            string filename = Path.GetFileName(absolutePath);
            string folder   = Path.GetDirectoryName(absolutePath);
            try
            {
                return SaveClassToXMLOutsideData(classToSave, folder, filename, overWrite);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //Further handels checking of parameters and does the actual saving.
        public static bool _SaveClassToXML(object classToSave, string folder, string fileName, bool overWrite, bool isRelative)
        {
            //check object
            if (classToSave == null)
            {
                throw new ArgumentException("XML_to_Class | SaveClassToXML | Given object is null.");
            }
            //check file name
            if (!Path.GetDirectoryName(fileName).Equals(string.Empty))
            {
                throw new ArgumentException("XML_to_Class | SaveClassToXML | fileName has a derectory attached to it, please use the overload method if you wish to use a full file path.");
            }
            string extention = Path.GetExtension(Path.GetExtension(fileName));
            if (extention.Equals(string.Empty))
            {
                fileName += ".xml";
            }
            else if (!extention.Equals(".xml"))
            {
                throw new ArgumentException("XML_to_Class | SaveClassToXML | fileName has a non XML extention. This method is for saving to XML only.");
            }
            //check folder
            if (!Path.GetExtension(folder).Equals(string.Empty))
            {
                throw new ArgumentException("XML_to_Class | SaveClassToXML | folder has an extention. please use the overload method if you wish to use a full file path.");
            }
            if (!Directory.Exists(Application.dataPath + Path.DirectorySeparatorChar + folder))
            {
                Debug.LogWarning("XML_to_Class | SaveClassToXML | Saving class to a folder that doesn't exsist, folder will be created.");
                Directory.CreateDirectory(Application.dataPath + Path.DirectorySeparatorChar + folder);
            }

            //Turn relative path into the absolute path
            string absoluteFilePath = Application.dataPath + Path.DirectorySeparatorChar + folder + Path.DirectorySeparatorChar + fileName;
            if (File.Exists(absoluteFilePath) && !overWrite)
            {
                return false; //file already exsists and we are not allowed to overwrite it.
            }

            XmlSerializer s      = new XmlSerializer(classToSave.GetType());
            FileStream    stream = new FileStream(absoluteFilePath, FileMode.Create, FileAccess.Write);
            s.Serialize(stream, classToSave);
            stream.Dispose();
            return true;
        }
    }
}