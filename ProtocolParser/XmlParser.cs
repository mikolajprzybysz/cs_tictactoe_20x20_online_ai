using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Xml;

   public class XmlParser
    {
        public static string ToXML(object objToSerialize) 
        {
	        XmlSerializer serializer = null;
	        FileStream    stream = null;
            string xml;

	        try
	        {
                XmlWriterSettings settings = new XmlWriterSettings();
                //settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;

                MemoryStream ms = new MemoryStream();
		        StreamWriter output = new StreamWriter(ms);

                using (XmlWriter writer = XmlWriter.Create(output, settings))
                {
                    serializer = new XmlSerializer(objToSerialize.GetType());
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);
                    serializer.Serialize(writer, objToSerialize, namespaces);

                    writer.Flush();
                    writer.Close();
                }

                using (StreamReader sr = new StreamReader(ms))
                {
                    ms.Position = 0;
                    xml = sr.ReadToEnd();
                    sr.Close();
                }
                
		        /*output.NewLine=String.Empty;
		        serializer = new XmlSerializer(objToSerialize.GetType());
                
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                
                
		        serializer.Serialize(output, objToSerialize,namespaces);*/
		        return xml;
	        }

	        catch
	        {
	            return ""; // return indication for error
	        }

	        finally
	        {
		        if(stream != null)
		        {
			        stream.Close();
		        }
	        }
        }
        /*
        public static protocol parseXml(String path)
        {
            protocol myMessage = null;
            try{
                TextReader reader = new StreamReader(path);
                XmlSerializer serializer = new XmlSerializer(typeof(protocol));
                myMessage = (protocol)serializer.Deserialize(reader);
                reader.Close();
            }catch(System.Xml.XmlException xe){
                MessageBox.Show (xe.Message, "XML Parse Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }catch(InvalidOperationException ioe){
                MessageBox.Show (ioe.InnerException.Message, "XML Serialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return myMessage;
        }        
        */
        public static message parseXml(String msg)
        {
            
            message myMessage = null;
            //int start= message.IndexOf("<message");
            //message = message.Substring(start,message.Length -start);
            //message = "<?xml version=\"1.0\" encoding=\"utf-8\"?><protocol>" + message + "</protocol>";

            try
            {
                TextReader reader = new StringReader(msg);
                XmlSerializer serializer = new XmlSerializer(typeof(message));
                myMessage = (message)serializer.Deserialize(reader);
                
                reader.Close();
            }
            catch (System.Xml.XmlException xe)
            {
                MessageBox.Show(xe.Message, "XML Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException ioe)
            {
                MessageBox.Show(ioe.InnerException.Message, "XML Serialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return myMessage;
        }
        /*
        public static protocol parseXml(String message)
        {
            XmlTextReader textReader = new XmlTextReader(new StringReader(message));
            textReader.Read();
            // If the node has value
            protocol
            if (textReader.Read() != null)
            {

            }
            while (textReader.Read())
            {
                // Move to fist element
                textReader.MoveToElement();
                Console.WriteLine("XmlTextReader Properties Test");
                Console.WriteLine("===================");
                // Read this element's properties and display them on console
                Console.WriteLine("Name:" + textReader.Name);
                Console.WriteLine("Base URI:" + textReader.BaseURI);
                Console.WriteLine("Local Name:" + textReader.LocalName);
                Console.WriteLine("Attribute Count:" + textReader.AttributeCount.ToString());
                Console.WriteLine("Depth:" + textReader.Depth.ToString());
                Console.WriteLine("Line Number:" + textReader.LineNumber.ToString());
                Console.WriteLine("Node Type:" + textReader.NodeType.ToString());
                Console.WriteLine("Attribute Count:" + textReader.Value.ToString());

            }
        }
         * */
         
    }


