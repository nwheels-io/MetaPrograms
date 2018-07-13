using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CommonExtensions;
using Example.WebUIModel.Metadata;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.Fluent.Generator;

namespace Example.HyperappAdapter
{
    public class PageModuleGenerator
    {
        private readonly WebPageMetadata _metadata;
        private readonly Dictionary<TypeMember, LocalVariable> _serviceVarByType;
        private LocalVariable _appVariable;
        private LocalVariable _formVariable;

        public PageModuleGenerator(WebPageMetadata metadata)
        {
            _metadata = metadata;
            _serviceVarByType = new Dictionary<TypeMember, LocalVariable>();
        }

        public ModuleMember GenerateModule()
        {
            DetermineModulePath(_metadata, out string[] folderPath, out string moduleName);
            
            return MODULE(folderPath, moduleName, () => {
                WriteImports();
                WritePageState();
                WritePageActions();
                WritePageView();
                WriteControllerInitializer();
            });
        }

        private void WriteImports()
        {
            IMPORT.MODULE("babel-polyfill");
            IMPORT.TUPLE("app", out _appVariable, "h", out var @h).FROM("hyperapp");
            IMPORT.TUPLE("Form", out _formVariable).FROM("./components/form");

            foreach (var api in _metadata.BackendApis)
            {
                IMPORT.TUPLE($"{api.ServiceName}Service", out var @service).FROM($"./services/{api.ServiceName}Service");
                _serviceVarByType.Add(api.InterfaceType, @service);
            }
        }

        private void WritePageState()
        {
            FINAL("PageState", out var @ref, INITOBJECT(() => {
                KEY("model", INITOBJECT(() => {
                    foreach (var property in _metadata.StateClass.Members.OfType<PropertyMember>())
                    {
                        KEY(property.Name, NULL);
                    }
                }));
                KEY("form", USE(_formVariable).DOT("createState").INVOKE());
            }));
        }

        private void WritePageActions()
        {
            FINAL("PageActions", out var @ref, INITOBJECT(() => {
                KEY("getState", LAMBDA(() 
                    => DO.RETURN(LAMBDA((@state, @actions) 
                        => DO.RETURN(@state)))));
                
                KEY("replaceModel", LAMBDA(@newModel  
                    => DO.RETURN(LAMBDA((@state, @actions) 
                        => DO.RETURN(INITOBJECT(("model", @newModel)))
                    ))
                ));
                
                //KEY("form", USE("Form").DOT("createActions").INVOKE());
                
            }));
        }

        private void WritePageView()
        {
            FINAL("PageView", out var @ref, LAMBDA(() => {
                PARAMETER(TUPLE("model", out var @model));
                PARAMETER("actions", out var @actions);

                DO.RETURN(XML(BuildViewXml(@model, @actions)));
            }));
        }

        private XElement BuildViewXml(AbstractExpression @model, AbstractExpression @actions)
        {
            return new XElement("Form.component", 
                new XElement("Form.submit")
            );
        }

        private void WriteControllerInitializer()
        {
            FINAL("controller", out var @controller, USE(_appVariable).INVOKE(
                USE("PageState"),
                USE("PageActions"),
                USE("PageView"),
                USE("document").DOT("body")
            ));
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