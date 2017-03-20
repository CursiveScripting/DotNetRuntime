﻿using Cursive;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace CursiveCSharpBackend.Services
{
    internal static class WorkspaceSavingService
    {
        internal static XmlDocument WriteDefinition(Workspace workspace)
        {
            var doc = new XmlDocument();

            var root = doc.CreateElement("Workspace");
            doc.AppendChild(root);

            Queue<DataType> typesToWrite = new Queue<DataType>(workspace.TypesByName.Values);
            HashSet<DataType> typesWritten = new HashSet<DataType>();

            while (typesToWrite.Any())
            {
                DataType type = typesToWrite.Dequeue();

                if (type.Extends != null && !typesWritten.Contains(type.Extends))
                {
                    typesToWrite.Enqueue(type);
                    continue;
                }

                WriteType(root, type);
                typesWritten.Add(type);
            }

            foreach (var kvp in workspace.RequiredProcesses)
            {
                WriteProcess(workspace, kvp.Value, kvp.Key, root, "RequiredProcess");
            }

            foreach (var kvp in workspace.Processes)
            {
                var process = kvp.Value;
                if (!(process is SystemProcess))
                    continue;

                WriteProcess(workspace, process, kvp.Key, root, "SystemProcess");
            }

            return doc;
        }

        public static string ToHexString(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        private static void WriteType(XmlElement parent, DataType type)
        {
            var doc = parent.OwnerDocument;
            var node = doc.CreateElement("Type");
            node.Attributes.Append(CreateAttribute(doc, "name", type.Name));
            node.Attributes.Append(CreateAttribute(doc, "color", type.Color.ToHexString()));

            if (type.Validation != null)
            {
                var regex = type.Validation.ToString();
                node.Attributes.Append(CreateAttribute(doc, "validation", regex));
            }

            if (type.Extends != null)
            {
                node.Attributes.Append(CreateAttribute(doc, "extends", type.Extends.Name));
            }

            if (!string.IsNullOrEmpty(type.Guidance))
            {
                node.Attributes.Append(CreateAttribute(doc, "guidance", type.Guidance));
            }

            parent.AppendChild(node);
        }

        private static void WriteProcess(Workspace workspace, Process process, string processName, XmlElement parent, string elementName)
        {
            var doc = parent.OwnerDocument;
            var processNode = doc.CreateElement(elementName);
            processNode.Attributes.Append(CreateAttribute(doc, "name", processName));
            parent.AppendChild(processNode);

            if (!string.IsNullOrEmpty(process.Folder))
                processNode.Attributes.Append(CreateAttribute(doc, "folder", process.Folder));

            if (!string.IsNullOrEmpty(process.Description))
            {
                var descNode = doc.CreateElement("Description");
                descNode.InnerText = process.Description;
                processNode.AppendChild(descNode);
            }

            if (process.Inputs != null)
                foreach (var input in process.Inputs)
                {
                    var inputNode = doc.CreateElement("Input");
                    inputNode.Attributes.Append(CreateAttribute(doc, "name", input.Name));

                    var type = input.Type;
                    inputNode.Attributes.Append(CreateAttribute(doc, "type", type.Name));
                    processNode.AppendChild(inputNode);
                }

            if (process.Outputs != null)
                foreach (var output in process.Outputs)
                {
                    var outputNode = doc.CreateElement("Output");
                    outputNode.Attributes.Append(CreateAttribute(doc, "name", output.Name));

                    var type = output.Type;
                    outputNode.Attributes.Append(CreateAttribute(doc, "type", type.Name));
                    processNode.AppendChild(outputNode);
                }

            if (process.ReturnPaths != null)
                foreach (var path in process.ReturnPaths)
                {
                    var pathNode = doc.CreateElement("ReturnPath");
                    pathNode.Attributes.Append(CreateAttribute(doc, "name", path));
                    processNode.AppendChild(pathNode);
                }
        }

        private static XmlAttribute CreateAttribute(XmlDocument doc, string name, string value)
        {
            var attr = doc.CreateAttribute(name);
            attr.Value = value;
            return attr;
        }
    }
}
