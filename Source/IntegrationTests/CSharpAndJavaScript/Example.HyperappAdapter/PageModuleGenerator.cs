using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CommonExtensions;
using Example.HyperappAdapter.Components;
using Example.HyperappAdapter.Metadata;
using Example.WebUIModel.Metadata;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.Fluent.Generator;

namespace Example.HyperappAdapter
{
    public class PageModuleGenerator
    {
        private readonly WebPageMetadata _metadata;
        private readonly IComponentAdapterFactory _componentAdapterFactory;
        private readonly HyperappPageMetadata _metaPageExtension;
        private readonly List<IComponentAdapter> _components;
        private LocalVariable _appVariable;
        private LocalVariable _formVariable;

        public PageModuleGenerator(WebPageMetadata metadata, IComponentAdapterFactory componentAdapterFactory)
        {
            _metadata = metadata;
            _componentAdapterFactory = componentAdapterFactory;
            _components = new List<IComponentAdapter>();
            _metaPageExtension = new HyperappPageMetadata();

            metadata.Extensions.Add(_metaPageExtension);
        }

        public ModuleMember GenerateModule()
        {
            CreateComponentAdapters();
            DetermineModulePath(_metadata, out string[] folderPath, out string moduleName);
            
            return MODULE(folderPath, moduleName, () => {
                WriteImports();
                WritePageState();
                WritePageActions();
                WritePageView();
                WriteControllerInitializer();
            });
        }

        private void CreateComponentAdapters()
        {
            foreach (var component in _metadata.Components)
            {
                _components.Add(_componentAdapterFactory.CreateComponentAdapter(component));
            }
        }

        private void WriteImports()
        {
            IMPORT.MODULE("babel-polyfill");
            IMPORT.TUPLE("app", out _appVariable, "h", out var @h).FROM("hyperapp");
            IMPORT.TUPLE("Form", out _formVariable).FROM("./components/form");

            foreach (var api in _metadata.BackendApis)
            {
                var suffixedServiceName = api.ServiceName.AppendSuffixFragments("Service");

                IMPORT
                    .TUPLE(suffixedServiceName.ToString(CasingStyle.Pascal), out var @service)
                    .FROM($"./services/{suffixedServiceName.ToString(CasingStyle.Kebab)}");

                _metaPageExtension.ServiceVarByType.Add(api.InterfaceType, @service);
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

                _components.ForEach(c => c.GenerateStateKeys());
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

                _components.ForEach(c => c.GenerateActionKeys());
            }));
        }

        private void WritePageView()
        {
            FINAL("PageView", out var @ref, LAMBDA(() => {
                PARAMETER(TUPLE("model", out var @model));
                PARAMETER("actions", out var @actions);

                var viewJsx = BuildViewJsx(@model, @actions);
                DO.RETURN(viewJsx != null ? XML(viewJsx) : (AbstractExpression)NULL);
            }));
        }

        private XElement BuildViewJsx(AbstractExpression @model, AbstractExpression @actions)
        {
            var rootComponent = _components.FirstOrDefault();
            return rootComponent?.GenerateViewMarkup();
        }

        private void WriteControllerInitializer()
        {
            FINAL("PageController", out var @controller, USE(_appVariable).INVOKE(
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
                moduleName = page.PageClass.Name.TrimSuffixFragment("Page").ToString().ToLower();
            }
        }
    }
}