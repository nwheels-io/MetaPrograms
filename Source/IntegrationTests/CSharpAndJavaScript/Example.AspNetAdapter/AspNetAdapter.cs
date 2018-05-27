using Example.WebUIModel.Metadata;
using MetaPrograms.Adapters.Roslyn.Writer;
using MetaPrograms.CodeModel.Imperative;

namespace Example.AspNetAdapter
{
    public class AspNetAdapter
    {
        private readonly ICodeGeneratorOutput _output;

        public AspNetAdapter(ICodeGeneratorOutput output)
        {
            _output = output;
        }

        public void GenerateImplementations(WebUIMetadata ui)
        {
            var writer = new RoslynCodeModelWriter(_output);

            using (var session = CodeGenerator.NewSession())
            {
                GenerateInfrastructureTypes();
                GenerateControllerTypes(ui);

                writer.AddCodeModel(session.GetCodeModel());
            }

            writer.WriteAll();
        }

        private static void GenerateControllerTypes(WebUIMetadata ui)
        {
            foreach (var api in ui.BackendApis)
            {
                CodeGenerator.NAMESPACE($"{api.InterfaceType.Namespace}.WebApi", () => {
                    WebApiControllerGenerator.WebApiController(api);
                });
            }
        }

        private static void GenerateInfrastructureTypes()
        {
            CodeGenerator.NAMESPACE(typeof(AspNetMiddlewareGenerator).Namespace, () => {
                AspNetMiddlewareGenerator.InvalidModelAutoResponderAttribute();
            });
        }
    }
}
