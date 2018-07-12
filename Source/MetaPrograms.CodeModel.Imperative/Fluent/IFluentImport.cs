using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public interface IFluentImport : IFluentImportTuple
    {
        void MODULE(string moduleName);
        void MODULE(ModuleMember module);
        IFluentImportAs ALL { get; }
        IFluentImportTuple DEFAULT(string name, out LocalVariable @var);
    }

    public interface IFluentImportAs
    {
        IFluentImportFrom AS(string fieldName, out LocalVariable @var);
    }

    public interface IFluentImportTuple : IFluentImportFrom
    {
        IFluentImportFrom TUPLE(string name1, out LocalVariable @var1);
        IFluentImportFrom TUPLE(string name1, out LocalVariable @var1, string name2, out LocalVariable @var2);
        IFluentImportFrom TUPLE(string name1, out LocalVariable @var1, string name2, out LocalVariable @var2, string name3, out LocalVariable @var3);
        IFluentImportFrom TUPLE(string[] names, out LocalVariable[] vars);
    }

    public interface IFluentImportFrom
    {
        void FROM(string moduleName);
        void FROM(ModuleMember module);
    }
}
