using System;
using System.Collections.Generic;
using System.Threading;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative
{
    public class CodeGeneratorContext : IDisposable
    {
        private static readonly AsyncLocal<CodeGeneratorContext> Current = new AsyncLocal<CodeGeneratorContext>();
        private readonly Stack<object> _stateStack = new Stack<object>();
        private readonly List<AbstractMember> _generatedMembers = new List<AbstractMember>();
        private readonly Dictionary<object, AbstractMember> _generatedMembersByBindings = new Dictionary<object, AbstractMember>();
        
        public CodeGeneratorContext(ImmutableCodeModel originalCodeModel)
        {
            if (Current.Value != null)
            {
                throw new InvalidOperationException(
                    "Another instance of CodeGeneratorContext is already associated with the current call context.");
            }

            Current.Value = this;
            this.OriginalCodeModel = originalCodeModel;
        }

        public void Dispose()
        {
            if (Current.Value == this)
            {
                Current.Value = null;
            }
        }

        public IDisposable PushState(object state)
        {
            return new StackStateScope(_stateStack, state);
        }

        public void AddGeneratedMember(AbstractMember member)
        {
            _generatedMembers.Add(member);

            foreach (var binding in member.Bindings)
            {
                _generatedMembersByBindings.Add(binding, member);
            }
        }

        public TMember TryFindMember<TMember>(object binding)
            where TMember : AbstractMember
        {
            AbstractMember member;

            if (OriginalCodeModel.MembersByBndings.TryGetValue(binding, out member))
            {
                return (TMember)member;
            }
            
            if (_generatedMembersByBindings.TryGetValue(binding, out member))
            {
                return (TMember)member;
            }

            return null;
        }

        public TMember FindMemberOrThrow<TMember>(object binding)
            where TMember : AbstractMember
        {
            var member = TryFindMember<TMember>(binding);

            if (member != null)
            {
                return member;
            }
            
            throw new KeyNotFoundException($"Could not find '{typeof(TMember).Name}' with binding '{binding}'.");
        }

        public ImmutableCodeModel OriginalCodeModel { get; }

        public ImmutableCodeModel GetGeneratedCodeModel() => new ImmutableCodeModel(_generatedMembers);
        
        public static CodeGeneratorContext CurrentContext => Current.Value;

        public static CodeGeneratorContext GetContextOrThrow() => 
            CurrentContext ?? 
            throw new InvalidOperationException(
                "No CodeGeneratorContext exists in the current call context. " +
                "Code generation operations require a current CodeGeneratorContext. " +
                "Instantiate CodeGeneratorContext before any code generation operations, and Dispose it afterwards.");

        private class StackStateScope : IDisposable
        {
            private readonly Stack<object> _stateStack;
            private readonly object _state;

            public StackStateScope(Stack<object> stateStack, object state)
            {
                _stateStack = stateStack;
                _state = state;
                _stateStack.Push(state);
            }

            public void Dispose()
            {
                if (_stateStack.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"Code generator state stack mismatch: attempted to pop '{_state}', but the stack is empty.");
                }
                
                if (_stateStack.Peek() != _state)
                {
                    throw new InvalidOperationException(
                        $"Code generator state stack mismatch: attempted to pop '{_state}', but the top item is '{_stateStack.Peek()}'.");
                }

                _stateStack.Pop();
            }
        }
    }
}