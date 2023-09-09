using System;
using System.Collections.Generic;
using System.Reflection;

namespace RedSaw.Brass{

    public enum BrassValueType: byte{

        Int,
        Float,
        String,
        Bool,
        Array,
    }
    public enum BrassTokenType: byte{

        Dict,
        Array,
        Value,
    }
    public enum BrassMemberType: byte{

        FieldInfo,
        PropertyInfo,
        MethodInfo,
    }


    public abstract class BrassToken{

        public abstract BrassTokenType BrassType{ get; }
        public readonly BrassMemberType memberType;
        public readonly string key;
        public readonly string variableName;
        public readonly Type valueType;
        public readonly Type declearType;
        
        public BrassToken(string key, string variableName, Type valueType, Type declearType, BrassMemberType memberType){

            this.key = key;
            this.variableName = variableName;
            this.valueType = valueType;
            this.declearType = declearType;
            this.memberType = memberType;
        }
        public object GetValue(object instance){

            if(instance == null || instance.GetType() != valueType)return null;
            return memberType switch
            {
                BrassMemberType.FieldInfo => declearType.GetField(variableName)?.GetValue(instance),
                BrassMemberType.PropertyInfo => declearType.GetProperty(variableName)?.GetValue(instance),
                BrassMemberType.MethodInfo => declearType.GetMethod(variableName)?.Invoke(instance, null),
                _ => null,
            };
        }
    }

    public class BrassDictionary : BrassToken
    {
        public override BrassTokenType BrassType => BrassTokenType.Dict;
        public readonly Dictionary<string, BrassToken> children = new();
        public BrassDictionary(string key, string variableName, Type valueType, Type declearType, BrassMemberType memberType) : base(key, variableName, valueType, declearType, memberType)
        {

        }
    }
    public class BrassValue : BrassToken{

        public override BrassTokenType BrassType => BrassTokenType.Value;
        public readonly BrassValueType brassValueType;
        public BrassValue(string key, string variableName, Type valueType, Type declearType, BrassMemberType memberType, BrassValueType brassValueType) : base(key, variableName, valueType, declearType, memberType)
        {
            this.brassValueType = brassValueType;
        }
    }

}