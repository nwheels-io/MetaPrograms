using System;

namespace MetaPrograms.Members
{
    public struct TypeGeneratorInfo
    {
        public TypeGeneratorInfo(Type factoryType, TypeKey? typeKey, TypeMember activationContract)
        {
            FactoryType = factoryType;
            TypeKey = typeKey;
            ActivationContract = activationContract;
        }

        public Type FactoryType { get; }
        public TypeKey? TypeKey { get; }

        // return type of factory interface
        public TypeMember ActivationContract { get; }
    }
}
