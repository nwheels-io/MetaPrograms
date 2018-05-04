﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class MethodSignature
    {
        public MethodSignature(
            bool isAsync, 
            MethodParameter returnValue,
            ImmutableList<MethodParameter> parameters)
        {
            IsAsync = isAsync;
            ReturnValue = returnValue;
            Parameters = parameters;
        }

        public MethodSignature(
            MethodSignature source,
            Mutator<bool>? isAsync = null,
            Mutator<MethodParameter>? returnValue = null,
            Mutator<ImmutableList<MethodParameter>>? parameters = null)
        {
            IsAsync = isAsync.MutatedOrOriginal(source.IsAsync);
            ReturnValue = returnValue.MutatedOrOriginal(source.ReturnValue);
            Parameters = parameters.MutatedOrOriginal(source.Parameters);
        }

        public bool IsVoid => (ReturnValue == null);

        public bool IsAsync { get; }
        public MethodParameter ReturnValue { get; }
        public ImmutableList<MethodParameter> Parameters { get; }
    }
}
