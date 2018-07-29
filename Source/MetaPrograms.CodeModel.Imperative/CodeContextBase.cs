using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Fluent;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative
{
    public abstract class CodeContextBase
    {
        private readonly Stack<object> _stateStack = new Stack<object>();

        public ImperativeCodeModel CodeModel { get; }
        public IClrTypeResolver ClrTypeResolver { get; }

        protected CodeContextBase(ImperativeCodeModel codeModel, IClrTypeResolver typeResolver)
        {
            this.CodeModel = codeModel;
            this.ClrTypeResolver = typeResolver;
        }
        
        public IDisposable PushState(object state)
        {
            if (state != null)
            {
                return new StackStateScope(_stateStack, state, pop: () => PopStateOrThrow(state.GetType()));
            }

            return null;
        }

        public TState PopStateOrThrow<TState>()
        {
            return (TState)PopStateOrThrow(typeof(TState));
        }

        public TState PeekStateOrThrow<TState>()
        {
            return (TState)PeekStateOrThrow(typeof(TState));
        }

        public bool TryPeekState<TState>(out TState state)
        {
            if (_stateStack.Count > 0 && _stateStack.Peek() is TState foundState)
            {
                state = foundState;
                return true;
            }

            state = default;
            return false;
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
            return TryLookupState<TypeMember>();
        }

        public ModuleMember TryGetCurrentModule()
        {
            return TryLookupState<ModuleMember>();
        }

        public TypeMember GetCurrentType()
        {
            return LookupStateOrThrow<TypeMember>();
        }

        public AbstractMember GetCurrentMember()
        {
            return LookupStateOrThrow<AbstractMember>();
        }

        public BlockContext GetCurrentBlock()
        {
            return LookupStateOrThrow<BlockContext>();
        }

        public bool TryFindMember<TMember>(object binding, out TMember member)
            where TMember : AbstractMember
        {
            if (binding != null && CodeModel.MembersByBndings.TryGetValue(binding, out var untypedMember))
            {
                member = (TMember)untypedMember;
                return true;
            }

            member = default;
            return false;
        }

        public TMember FindMemberOrThrow<TMember>(object binding)
            where TMember : AbstractMember
        {
            if (TryFindMember<TMember>(binding, out var member))
            {
                return member;
            }
            
            throw new KeyNotFoundException($"Could not find '{typeof(TMember).Name}' with binding '{binding}'.");
        }

        public TypeMember FindType<T>()
        {
            return FindType(typeof(T));
        }

        public TypeMember FindType(Type clrType)
        {
            if (clrType == null)
            {
                return null;
            }
            
            TypeMember typeMember;

            if (TryFindMember(binding: clrType, out typeMember))
            {
                if (typeMember.Status == MemberStatus.Incomplete)
                {
                    ClrTypeResolver.Complete(typeMember, this.CodeModel);
                }
            }
            else
            {
                typeMember = ClrTypeResolver.Resolve(clrType, this.CodeModel, distance: 0);
            }

            return typeMember;
        }

        public AbstractExpression GetConstantExpression(object value)
        {
            return AbstractExpression.FromValue(value, resolveType: this.FindType);
        }

        private object PopStateOrThrow(Type stateType)
        {
            var state = PeekStateOrThrow(stateType);

            _stateStack.Pop();

            if (state is IDisposable disposable)
            {
                disposable.Dispose();
            }
            
            Debug.WriteLine($"CODE GENERATOR CONTEXT >> PopStateOrThrow >> POP {((Object)state).GetType().Name} >> COUNT = {_stateStack.Count}");

            return state;
        }

        private object PeekStateOrThrow(Type stateType)
        {
            if (_stateStack.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Generator stack mismatch: expected '{stateType.Name}', but the stack is empty.");
            }

            var stateOnTop = _stateStack.Peek();
            
            if (!stateType.IsInstanceOfType(stateOnTop))
            {
                throw new InvalidOperationException(
                    $"Generator stack mismatch: expected '{stateType.Name}', but found '{stateOnTop.GetType().Name}'.");
            }

            return stateOnTop;
        }

        private class StackStateScope : IDisposable
        {
            private readonly Stack<object> _stateStack;
            private readonly object _state;
            private readonly Action _pop;

            public StackStateScope(Stack<object> stateStack, object state, Action pop)
            {
                _stateStack = stateStack;
                _state = state;
                _pop = pop;
                _stateStack.Push(state);
                Debug.WriteLine($"CODE CONTEXT >> StackStateScope.ctor >> PUSH {_state.GetType().Name} >> COUNT = {_stateStack.Count}");
            }

            public void Dispose()
            {
                _pop();
                Debug.WriteLine($"CODE CONTEXT >> StackStateScope.Dispose >> POP {_state.GetType().Name} >> COUNT = {_stateStack.Count}");
            }
        }
    }
}