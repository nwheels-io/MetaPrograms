using System.Linq;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class FluentImport : IFluentImport, IFluentImportAs, IFluentImportTuple, IFluentImportFrom
    {
        public ImportDirective Directive { get; } = new ImportDirective { };

        public IFluentImportTuple DEFAULT(string name, out LocalVariable @var)
        {
            Directive.AsDefault = new LocalVariable {
                Name = name
            };

            @var = Directive.AsDefault;
            return this;
        }

        public IFluentImportFrom AS(string name, out LocalVariable @var)
        {
            Directive.AsNamespace = new LocalVariable {
                Name = name
            };

            @var = Directive.AsNamespace;
            return this;
        }

        public IFluentImportAs ALL => this;

        public IFluentImportFrom TUPLE(string name1, out LocalVariable var1)
        {
            Directive.AsTuple = new TupleExpression(new LocalVariable {
                Name = name1
            });

            var1 = Directive.AsTuple.Variables[0];
            return this;
        }

        public IFluentImportFrom TUPLE(string name1, out LocalVariable var1, string name2, out LocalVariable var2)
        {
            Directive.AsTuple = new TupleExpression(
                new LocalVariable { Name = name1 },
                new LocalVariable { Name = name2 }
            );

            var1 = Directive.AsTuple.Variables[0];
            var2 = Directive.AsTuple.Variables[1];
            return this;
        }

        public IFluentImportFrom TUPLE(string name1, out LocalVariable var1, string name2, out LocalVariable var2, string name3, out LocalVariable var3)
        {
            Directive.AsTuple = new TupleExpression(
                new LocalVariable { Name = name1 },
                new LocalVariable { Name = name2 },
                new LocalVariable { Name = name3 }
            );

            var1 = Directive.AsTuple.Variables[0];
            var2 = Directive.AsTuple.Variables[1];
            var3 = Directive.AsTuple.Variables[2];
            return this;
        }

        public IFluentImportFrom TUPLE(string[] names, out LocalVariable[] vars)
        {
            Directive.AsTuple = new TupleExpression(names.Select(name => new LocalVariable { Name = name }).ToArray());

            vars = Directive.AsTuple.Variables.ToArray();
            return this;
        }

        public void FROM(string moduleName)
        {
            Directive.FromModuleName = moduleName;
            FlushImportDirective();
        }

        public void FROM(TypeMember module)
        {
            Directive.FromModule = module;
            FlushImportDirective();
        }

        private void FlushImportDirective()
        {
            var context = CodeGeneratorContext.GetContextOrThrow();
            var module = context.LookupStateOrThrow<ModuleMember>();

            module.Imports.Add(Directive);
        }
    }
}
