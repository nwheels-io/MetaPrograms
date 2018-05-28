using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using MetaPrograms.CodeModel.Imperative.Expressions;
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

        public TState PopStateOrThrow<TState>()
        {
            var state = PeekStateOrThrow<TState>();
            _stateStack.Pop();
            return state;
        }

        public TState PeekStateOrThrow<TState>()
        {
            if (_stateStack.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Code generator state stack mismatch: attempted to pop a {typeof(TState).Name}, but the stack is empty.");
            }

            if (!(_stateStack.Peek() is TState state))
            {
                throw new InvalidOperationException(
                    $"Code generator state stack mismatch: attempted to pop a {typeof(TState).Name}, " +
                    $"but the top item is a {_stateStack.Peek().GetType().Name}'.");
            }

            return state;
        }

        public TState TryLookupState<TState>()
        {
            return _stateStack.OfType<TState>().FirstOrDefault();
        }

        public TState LookupStateOrThrow<TState>()
        {
            var state = TryLookupState<TState>();

            if (state != null)
            {
                return state;
            }

            throw new InvalidOperationException($"Could not find a {typeof(TState).Name} down the state stack.");
        }

        public TypeMember TryGetCurrentType()
        {
            return TryLookupState<MemberRef<TypeMember>>().Get();
        }

        public TypeMember GetCurrentType()
        {
            return LookupStateOrThrow<MemberRef<TypeMember>>().Get();
        }

        public AbstractMember GetCurrentMember()
        {
            return LookupStateOrThrow<IMemberRef>().Get();
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

        public TypeMember FindType<T>()
        {
            return FindMemberOrThrow<TypeMember>(binding: typeof(T));
        }

        public TypeMember FindType(Type clrType)
        {
            return FindMemberOrThrow<TypeMember>(binding: clrType);
        }

        public AbstractExpression GetConstantExpression(object value)
        {
            if (value == null)
            {
                return new ConstantExpression(MemberRef<TypeMember>.Null, null);
            }

            if (value is AbstractExpression expr)
            {
                return expr;
            }

            var type = FindType(value.GetType());
            
            if (type.IsArray)
            {
                return new NewArrayExpression(
                    type.GetRef(), 
                    type.UnderlyingType, 
                    GetConstantExpression(((IList)value).Count),
                    ((IEnumerable)value).Cast<object>().Select(GetConstantExpression).ToImmutableList());
            }
            
            return new ConstantExpression(type.GetRef(), value);
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