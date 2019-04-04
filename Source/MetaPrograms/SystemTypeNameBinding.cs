using System;
using MetaPrograms.Members;

namespace MetaPrograms
{
    public class SystemTypeNameBinding : IEquatable<SystemTypeNameBinding>
    {
        public readonly string SystemTypeMetadataName;

        public SystemTypeNameBinding(Type clrType)
        {
            this.SystemTypeMetadataName = clrType.FullName + "," + clrType.Assembly.GetName().Name;
        }

        public SystemTypeNameBinding(string systemTypeMetadataName)
        {
            this.SystemTypeMetadataName = systemTypeMetadataName;
        }

        public override int GetHashCode()
        {
            return (SystemTypeMetadataName != null ? SystemTypeMetadataName.GetHashCode() : 0);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SystemTypeNameBinding) obj);
        }

        public bool Equals(SystemTypeNameBinding other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(SystemTypeMetadataName, other.SystemTypeMetadataName);
        }

        public override string ToString()
        {
            return SystemTypeMetadataName;
        }
    }
}
