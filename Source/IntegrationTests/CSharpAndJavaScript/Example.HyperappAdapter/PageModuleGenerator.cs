using System;
using System.Collections.Generic;
using System.Text;
using CommonExtensions;
using Example.WebUIModel.Metadata;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.Fluent.Generator;

namespace Example.HyperappAdapter
{
    public class PageModuleGenerator
    {
        public static ModuleMember PageModule(WebPageMetadata page)
        {
            DetermineModulePath(page, out string[] folderPath, out string moduleName);
            
            return MODULE(folderPath, moduleName, () => {
                IMPORT.MODULE("babel-polyfill");
                IMPORT.TUPLE("app", out var @app, "h", out var @h).FROM("hyperapp");
                IMPORT.TUPLE("Form", out var @form).FROM("./components/form");

                var serviceVarByType = new Dictionary<TypeMember, LocalVariable>();
                
                foreach (var api in page.BackendApis)
                {
                    IMPORT.TUPLE(api.ServiceName, out var @service).FROM($"./services/{api.ServiceName}");
                    serviceVarByType.Add(api.InterfaceType, @service);
                }
            });
        }

        private static void DetermineModulePath(WebPageMetadata page, out string[] folderPath, out string moduleName)
        {
            if (page.IsIndex)
            {
                folderPath = new[] { "src" };
                moduleName = "index";
            }
            else
            {
                folderPath = new[] { "src", "pages" };
                moduleName = page.PageClass.Name.TrimSuffix("Page").ToLower();
            }
        }
    }
}