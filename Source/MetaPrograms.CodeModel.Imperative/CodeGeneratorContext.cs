using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Fluent;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative
{
    public class CodeGeneratorContext : CodeContextBase, IDisposable
    {
        private static readonly AsyncLocal<CodeGeneratorContext> Current = new AsyncLocal<CodeGeneratorContext>();

        private readonly List<AbstractMember> _generatedMembers = new List<AbstractMember>();
        
        public CodeGeneratorContext(ImperativeCodeModel codeModel, IClrTypeResolver clrTypeResolver)
            : base(codeModel, clrTypeResolver)
        {
            if (Current.Value != null)
            {
                throw new InvalidOperationException(
                    "Another instance of CodeGeneratorContext is already associated with the current call context.");
            }

            Current.Value = this;
        }

        public void Dispose()
        {
            if (Current.Value == this)
            {
                Current.Value = null;
            }
        }

        public void AddGeneratedMember<TMember>(TMember member, bool isTopLevel)
            where TMember : AbstractMember
        {
            _generatedMembers.Add(member);
            this.CodeModel.Add(member, isTopLevel);
        }

        public IEnumerable<AbstractMember> GeneratedMembers => _generatedMembers;

        public static CodeGeneratorContext CurrentContext => Current.Value;

        public static CodeGeneratorContext GetContextOrThrow() => 
            CurrentContext ?? 
            throw new InvalidOperationException(
                "No CodeGeneratorContext exists in the current call context. " +
                "Code generation operations require a current CodeGeneratorContext. " +
                "Instantiate CodeGeneratorContext before any code generation operations, and Dispose it afterwards.");
    }

    public class CodeReaderContext : CodeContextBase, IDisposable
    {
        private static readonly AsyncLocal<CodeReaderContext> Current = new AsyncLocal<CodeReaderContext>();

        public CodeReaderContext(ImperativeCodeModel codeModel, IClrTypeResolver clrTypeResolver)
            : base(codeModel, clrTypeResolver)
        {
            if (Current.Value != null)
            {
                throw new InvalidOperationException(
                    "Another instance of CodeReaderContext is already associated with the current call context.");
            }

            Current.Value = this;
        }

        public void Dispose()
        {
            if (Current.Value == this)
            {
                Current.Value = null;
            }
        }

        public static CodeReaderContext CurrentContext => Current.Value;

        public static CodeReaderContext GetContextOrThrow() => 
            CurrentContext ?? 
            throw new InvalidOperationException(
                "No CodeReaderContext exists in the current call context. " +
                "Code reading operations require a current CodeReaderContext. " +
                "Instantiate CodeReaderContext before any code reading operations, and Dispose it afterwards.");
    }
}
