using System.Linq;
using System.Runtime.CompilerServices;
using Example.WebUIModel.Metadata;
using MetaPrograms.Adapters.Reflection.Reader;
using MetaPrograms.CSharp;
using MetaPrograms.CSharp.Writer;
using MetaPrograms;
using MetaPrograms.Members;
using static MetaPrograms.Fluent.Generator;

namespace Example.AspNetAdapter
{
    public class AspNetAdapter
    {
        private readonly ImperativeCodeModel _codeModel;
        private readonly ICodeGeneratorOutput _output;
        private CodeGeneratorContext _context;
        private TypeMember _middlewareType;

        public AspNetAdapter(ImperativeCodeModel codeModel, ICodeGeneratorOutput output)
        {
            _codeModel = codeModel;
            _output = output;
        }

        public void GenerateImplementations(WebUIMetadata ui)
        {
            using (_context = new CodeGeneratorContext(_codeModel, new ClrTypeResolver(), LanguageInfo.Entries.CSharp()))
            {
                GenerateInfrastructureTypes();
                GenerateControllerTypes(ui);

                var writer = new RoslynCodeModelWriter(_codeModel, _output);
                writer.AddMembers(_context.GeneratedMembers);
                writer.WriteAll();
            }
        }

        private void GenerateInfrastructureTypes()
        {
            NAMESPACE(typeof(AspNetMiddlewareGenerator).Namespace, () => {
                _middlewareType = AspNetMiddlewareGenerator.InvalidModelAutoResponderAttribute();
            });
        }

        private void GenerateControllerTypes(WebUIMetadata ui)
        {
            foreach (var api in ui.BackendApis)
            {
                NAMESPACE($"{api.InterfaceType.Namespace}.WebApi", () => {
                    WebApiControllerGenerator.WebApiController(_middlewareType, api);
                });
            }
        }
    }
}
