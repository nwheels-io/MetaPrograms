using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class FluentImport : IFluentImport, IFluentImportAs, IFluentImportTuple, IFluentImportFrom
    {
        public IFluentImportTuple DEFAULT(string name, out LocalVariable @var)
        {
            this.AsDefault = new LocalVariable(name, MemberRef<TypeMember>.Null);

            @var = this.AsDefault;
            return this;
        }

        public IFluentImportFrom AS(string name, out LocalVariable @var)
        {
            this.AsNamespace = new LocalVariable(name, MemberRef<TypeMember>.Null);

            @var = this.AsNamespace;
            return this;
        }

        public IFluentImportAs ALL => this;

        public IFluentImportFrom TUPLE(string name1, out LocalVariable var1)
        {
            this.AsTuple = new TupleExpression(new[] { new LocalVariable(name1, MemberRef<TypeMember>.Null) });

            var1 = this.AsTuple.Variables[0];
            return this;
        }

        public IFluentImportFrom TUPLE(string name1, out LocalVariable var1, string name2, out LocalVariable var2)
        {
            this.AsTuple = new TupleExpression(new[] {
                new LocalVariable(name1, MemberRef<TypeMember>.Null),
                new LocalVariable(name2, MemberRef<TypeMember>.Null),
            });

            var1 = this.AsTuple.Variables[0];
            var2 = this.AsTuple.Variables[1];
            return this;
        }

        public IFluentImportFrom TUPLE(string name1, out LocalVariable var1, string name2, out LocalVariable var2, string name3, out LocalVariable var3)
        {
            this.AsTuple = new TupleExpression(new[] {
                new LocalVariable(name1, MemberRef<TypeMember>.Null),
                new LocalVariable(name2, MemberRef<TypeMember>.Null),
                new LocalVariable(name3, MemberRef<TypeMember>.Null),
            });

            var1 = this.AsTuple.Variables[0];
            var2 = this.AsTuple.Variables[1];
            var3 = this.AsTuple.Variables[2];
            return this;
        }

        public IFluentImportFrom TUPLE(string[] names, out LocalVariable[] vars)
        {
            this.AsTuple = new TupleExpression(names.Select(name => new LocalVariable(name, MemberRef<TypeMember>.Null)));

            vars = this.AsTuple.Variables.ToArray();
            return this;
        }

        public void FROM(string moduleName)
        {
            this.FromModuleName = moduleName;
            FlushImportDirective();
        }

        public void FROM(MemberRef<TypeMember> module)
        {
            this.FromModule = module;
            FlushImportDirective();
        }

        public MemberRef<TypeMember> FromModule { get; private set; }
        public string FromModuleName { get; private set; }
        public LocalVariable AsDefault { get; private set; }
        public LocalVariable AsNamespace { get; private set; }
        public TupleExpression AsTuple { get; private set; }

        private void FlushImportDirective()
        {
            var context = CodeGeneratorContext.GetContextOrThrow();
            var typeBuilder = context.GetCurrentTypeBuilder();
            var directive = CreateImportDirective();

            typeBuilder.Imports.Add(directive);
        }

        private ImportDirective CreateImportDirective()
        {
            return new ImportDirective(
                fromModule: this.FromModule, 
                fromModuleName: this.FromModuleName, 
                asDefault: this.AsDefault, 
                asTuple: this.AsTuple, 
                asNamespace: this.AsNamespace);
        }
    }
}
