using System.Linq;
using System.Runtime.CompilerServices;
using Example.WebUIModel.Metadata;
using MetaPrograms.Adapters.Reflection.Reader;
using MetaPrograms.Adapters.Roslyn.Writer;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.Fluent.Generator;

namespace Example.AspNetAdapter
{
    public class AspNetAdapter
    {
        private readonly ImperativeCodeModel _codeModel;
        private readonly ICodeGeneratorOutput _output;
        private CodeGeneratorContext _context;
        private MemberRef<TypeMember> _middlewareType;

        public AspNetAdapter(ImperativeCodeModel codeModel, ICodeGeneratorOutput output)
        {
            _codeModel = codeModel;
            _output = output;
        }

        public void GenerateImplementations(WebUIMetadata ui)
        {
            using (_context = new CodeGeneratorContext(_codeModel, new ClrTypeResolver()))
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
                _middlewareType = AspNetMiddlewareGenerator.InvalidModelAutoResponderAttribute().GetRef();
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
