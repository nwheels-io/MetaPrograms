﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CommonExtensions;
using Example.WebUIModel.Metadata;
using MetaPrograms.Adapters.JavaScript;
using MetaPrograms.Adapters.JavaScript.Writer;
using MetaPrograms.Adapters.Reflection.Reader;
using MetaPrograms.CodeModel.Imperative;

namespace Example.HyperappAdapter
{
    public class HyperappAdapter
    {
        private static readonly string ClientSideFolderPath = 
            Path.Combine(Path.GetDirectoryName(typeof(HyperappAdapter).Assembly.Location), "ClientSide");

        private readonly ImperativeCodeModel _codeModel;
        private readonly ICodeGeneratorOutput _output;
        private readonly JavaScriptCodeWriter _codeWriter;

        public HyperappAdapter(ImperativeCodeModel codeModel, ICodeGeneratorOutput output)
        {
            _codeModel = codeModel;
            _output = output;
            _codeWriter = new JavaScriptCodeWriter(output);
        }

        public void GenerateImplementations(WebUIMetadata metadata)
        {
            CopyFrameworkFiles();

            using (new CodeGeneratorContext(_codeModel, new ClrTypeResolver()))
            {
                GenerateIndexHtml(metadata);
                GenerateBackendApiServices(metadata);
            }
        }

        private void CopyFrameworkFiles()
        {
            _output.AddSourceFile(new[] {"src", "components"}, "form.js", GetFrameworkFileContents("Components", "form.js"));
        }

        private void GenerateBackendApiServices(WebUIMetadata metadata)
        {
            foreach (var api in metadata.BackendApis)
            {
                GenerateBackendApiService(api);
            }
        }

        private void GenerateBackendApiService(WebApiMetadata api)
        {
            var module = BackendApiServiceGenerator.BackendApiService(api);
            _codeWriter.WriteModule(module);
        }

        private void GenerateIndexHtml(WebUIMetadata metadata)
        {
            var html = new XDocument(
                new XDocumentType("html", null, null, null),
                new XElement("html", 
                    new XElement("head",
                        new XElement("meta", new XAttribute("charset", "utf-8")),
                        new XElement("title", metadata.Pages.First().PageClass.Name.TrimSuffix("Page")),
                        new XElement("script",
                            new XAttribute("defer", true),
                            new XAttribute("src", "bundle.js"),
                            ""
                        )
                    ),
                    new XElement("body", "")
                )
            );

            _output.AddSourceFile(new[] {"build"}, "index.html", html.ToString());
        }

        private static string GetFrameworkFileContents(params string[] path)
        {
            var filePath = Path.Combine(ClientSideFolderPath, Path.Combine(path));
            return File.ReadAllText(filePath);
        }
    }
}
